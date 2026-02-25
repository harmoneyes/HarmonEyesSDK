using HarmonEyesSDK.DataModels.EyeTrackerDataModels;
using UnityEngine;

public class OculusEyeTrackingCollectingData : MonoBehaviour
{
    [SerializeField] Transform LeftEyeTransform;
    [SerializeField] Transform RightEyeTransform;

    [SerializeField] private OVRFaceExpressions FaceTrackingExpressions;
    private float leftEyeClosed;
    private float rightEyeClosed;

    public float defaultGazeDistance = 10f; // Default distance to project gaze if nothing is hit
    public Camera mainCamera;

    void Update()
    {
        if (FaceTrackingExpressions.ValidExpressions)
        {
            leftEyeClosed = FaceTrackingExpressions.GetWeight(OVRFaceExpressions.FaceExpression.EyesClosedL);
            rightEyeClosed = FaceTrackingExpressions.GetWeight(OVRFaceExpressions.FaceExpression.EyesClosedR);
        }

        long timeStamp = (long)((Time.timeAsDouble - EyeTrackingConfig.Instance.SessionStartTime));
        
        var eyeSample = EyeTrackingConfig.Instance.EyeTrackingAnalyzer.CreateSample(timeStamp, LeftEyeTransform,
            leftEyeClosed, RightEyeTransform, rightEyeClosed, mainCamera);

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

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(gazePosition);
       // Debug.Log($"Eye: {eyeTransform.name}, Screen Position: {screenPosition}"); //Used for debugging
    }
}
