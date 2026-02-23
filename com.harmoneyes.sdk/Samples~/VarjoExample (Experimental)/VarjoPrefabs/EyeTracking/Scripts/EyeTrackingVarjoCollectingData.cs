using HarmonEyesSDK.DataModels.EyeTrackerDataModels;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using Varjo.XR;


public class EyeTrackingVarjoAdvancedCollectingData : MonoBehaviour
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
    [Header("Varjo Settings")]

    private List<InputDevice> devices = new List<InputDevice>();
    private InputDevice device;
    private Eyes eyes;

    private VarjoEyeTracking.GazeData gazeData;
    private float leftEyePupilDiameter;
    private float rightEyePupilDiameter;

    [Header("Gaze data output frequency")]
    public VarjoEyeTracking.GazeOutputFrequency frequency;


    [Header("Gaze calibration settings")]
    public VarjoEyeTracking.GazeCalibrationMode gazeCalibrationMode = VarjoEyeTracking.GazeCalibrationMode.Fast;

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
    private void Start()
    {
        VarjoEyeTracking.SetGazeOutputFrequency(frequency);

        VarjoEyeTracking.RequestGazeCalibration(gazeCalibrationMode); //TODO may not needed ?

        //Hiding the gazetarget if gaze is not available or if the gaze calibration is not done
 /*       if (VarjoEyeTracking.IsGazeAllowed() && VarjoEyeTracking.IsGazeCalibrated())
        {
            gazeTarget.SetActive(true);
        }
        else
        {
            gazeTarget.SetActive(false);
        }

        if (showFixationPoint)
        {
            fixationPointTransform.gameObject.SetActive(true);
        }
        else
        {
            fixationPointTransform.gameObject.SetActive(false);
        }*/
    }

    void Update()
    {
        if (VarjoEyeTracking.IsGazeAllowed() && VarjoEyeTracking.IsGazeCalibrated())
        {
            //Get device if not valid
            if (!device.isValid)
            {
                GetDevice();
            }

            // Show gaze target
            //gazeTarget.SetActive(true); //TODO Enable gaze target if needed

            gazeData = VarjoEyeTracking.GetGaze();
            if (gazeData.status != VarjoEyeTracking.GazeStatus.Invalid)
            {
                var eyeMesurements = VarjoEyeTracking.GetEyeMeasurements();

                leftEyeOpened = eyeMesurements.leftEyeOpenness;
                rightEyeOpened = eyeMesurements.rightEyeOpenness;

                leftEyePupilDiameter = eyeMesurements.leftPupilDiameterInMM;
                rightEyePupilDiameter = eyeMesurements.rightPupilDiameterInMM;

                // GazeRay vectors are relative to the HMD pose so they need to be transformed to world space
                if (gazeData.leftStatus != VarjoEyeTracking.GazeEyeStatus.Invalid)
                {
                    LeftEyeTransform.position = mainCamera.transform.TransformPoint(gazeData.left.origin);
                    LeftEyeTransform.rotation = Quaternion.LookRotation(mainCamera.transform.TransformDirection(gazeData.left.forward));
                }

                if (gazeData.rightStatus != VarjoEyeTracking.GazeEyeStatus.Invalid)
                {
                    RightEyeTransform.position = mainCamera.transform.TransformPoint(gazeData.right.origin);
                    RightEyeTransform.rotation = Quaternion.LookRotation(mainCamera.transform.TransformDirection(gazeData.right.forward));
                }

                //TODO figure out if we need to use right origin or use the transformPoint also do we need to use their gaze implementation.
            }
        }

        //var leftEye = EyeTrackingConfig.Instance.EyeTrackingAnalyzer.AddEyeRecord(leftEyeOpened, LeftEyeTransform ,leftEyePupilDiameter);
        //var rightEye = EyeTrackingConfig.Instance.EyeTrackingAnalyzer.AddEyeRecord(rightEyeOpened, RightEyeTransform, rightEyePupilDiameter);

        long timeStamp = (long)((Time.timeAsDouble - EyeTrackingConfig.Instance.SessionStartTime));

        //var eyeSample = new Sample(timeStamp, leftEye, rightEye);

        //EyeTrackingConfig.Instance.EyeTrackingData.Samples.AddSample(eyeSample);
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
