using HarmonEyesSDK.DataModels.EyeTrackerDataModels;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using Varjo.XR;


public class EyeTrackingVarjoCollectingData : MonoBehaviour
{
    public Transform LeftEyeTransform;
    public Transform RightEyeTransform;

    private float leftEyeOpened;
    private float rightEyeOpened;

    public float defaultGazeDistance = 10f; // Default distance to project gaze if nothing is hit
    public Camera mainCamera;
    private RaycastHit hit;
    private Vector3 screenPosition;
    private Vector3 gazePosition;
    private Ray gazeRay;

    //Varjo Settings
    private List<InputDevice> devices = new List<InputDevice>();
    private InputDevice device;
    private Eyes eyes;
    private Vector3 leftEyePosition;
    private Vector3 rightEyePosition;
    private Quaternion leftEyeRotation;
    private Quaternion rightEyeRotation;

    void GetDevice()
    {
        InputDevices.GetDevicesAtXRNode(XRNode.CenterEye, devices);
        device = devices.FirstOrDefault();
    }

    void OnEnable()
    {
        if (!device.isValid)
        {
            GetDevice();
        }
    }

    void Update()
    {
        if (device.TryGetFeatureValue(CommonUsages.eyesData, out eyes))
        {
            eyes.TryGetLeftEyeOpenAmount(out leftEyeOpened);
            eyes.TryGetRightEyeOpenAmount(out rightEyeOpened);

            if (eyes.TryGetLeftEyePosition(out leftEyePosition))
            {
                LeftEyeTransform.localPosition = leftEyePosition;
            }

            if (eyes.TryGetLeftEyeRotation(out leftEyeRotation))
            {
                LeftEyeTransform.localRotation = leftEyeRotation;
            }

            if (eyes.TryGetRightEyePosition(out rightEyePosition))
            {
                RightEyeTransform.localPosition = rightEyePosition;
            }

            if (eyes.TryGetRightEyeRotation(out rightEyeRotation))
            {
                RightEyeTransform.localRotation = rightEyeRotation;
            }
        }

        var leftEye = EyeTrackingConfig.Instance.EyeTrackingAnalyzer.AddEyeRecord(leftEyeOpened, LeftEyeTransform);
        var rightEye = EyeTrackingConfig.Instance.EyeTrackingAnalyzer.AddEyeRecord(rightEyeOpened, RightEyeTransform);

        long timeStamp = (long)((Time.timeAsDouble - EyeTrackingConfig.Instance.SessionStartTime));

        var eyeSample = new Sample(timeStamp, leftEye, rightEye);

        EyeTrackingConfig.Instance.EyeTrackingData.Samples.AddSample(eyeSample);
    }

    void OnDrawGizmos()
    {
        if (LeftEyeTransform == null || RightEyeTransform == null || mainCamera == null) return;

        DrawGazeRayAndPosition(LeftEyeTransform, Color.red, Color.blue);

        DrawGazeRayAndPosition(RightEyeTransform, Color.green, Color.blue);
    }

    private void DrawGazeRayAndPosition(Transform eyeTransform, Color rayColor, Color hitColor)
    {
        Vector3 gazePosition;
        RaycastHit hit;

        Ray gazeRay = new Ray(eyeTransform.position, eyeTransform.forward);

        if (Physics.Raycast(gazeRay, out hit))
        {
            gazePosition = hit.point;
        }
        else
        {
            gazePosition = gazeRay.GetPoint(defaultGazeDistance);
        }

        // Draw the gaze ray
        Gizmos.color = rayColor;
        Gizmos.DrawLine(eyeTransform.position, gazePosition);

        // Draw a sphere at the gaze position
        Gizmos.color = hitColor;
        Gizmos.DrawWireSphere(gazePosition, 0.1f);

        screenPosition = mainCamera.WorldToScreenPoint(gazePosition);
        // Debug.Log($"Eye: {eyeTransform.name}, Screen Position: {screenPosition}"); //Used for debugging
    }
}
