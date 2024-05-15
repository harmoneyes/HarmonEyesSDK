using HarmonEyesSDK.DataModels.EyeTrackerDataModels;
using UnityEngine;
using Wave.Essence.Eye;

public class HTCViveEyeTrackingCollectingData : MonoBehaviour
{
    public float defaultGazeDistance = 10f; // Default distance to project gaze if nothing is hit
    public Camera mainCamera;

    private Vector3 leftEyePos;
    private Vector3 rightEyePos;

    private Vector3 leftEyeDir;
    private Vector3 rightEyeDir;

    private float leftPupilDiameter;
    private float rightPupilDiameter;

    private float eyeOpenLeft;
    private float eyeOpenRight;
    void Update()
    {
        EyeManager.Instance.GetLeftEyeOrigin(out leftEyePos);
        EyeManager.Instance.GetRightEyeOrigin(out rightEyePos);

        EyeManager.Instance.GetLeftEyeDirectionNormalized(out leftEyeDir);
        EyeManager.Instance.GetRightEyeDirectionNormalized(out rightEyeDir);

        EyeManager.Instance.GetLeftEyePupilDiameter(out leftPupilDiameter);
        EyeManager.Instance.GetRightEyePupilDiameter(out rightPupilDiameter);

        EyeManager.Instance.GetLeftEyeOpenness(out eyeOpenLeft);
        EyeManager.Instance.GetRightEyeOpenness(out eyeOpenRight);

        var leftEye = EyeTrackingConfig.Instance.EyeTrackingAnalyzer.AddEyeRecord(eyeOpenLeft, leftEyePos, leftEyeDir, leftPupilDiameter, mainCamera);
        var rightEye = EyeTrackingConfig.Instance.EyeTrackingAnalyzer.AddEyeRecord(eyeOpenRight, rightEyePos, rightEyeDir, rightPupilDiameter, mainCamera);

        long timeStamp = (long)((Time.timeAsDouble - EyeTrackingConfig.Instance.SessionStartTime));

        var eyeSample = new Sample(timeStamp, leftEye, rightEye);

        EyeTrackingConfig.Instance.EyeTrackingData.Samples.AddSample(eyeSample);
    }

    void OnDrawGizmos()
    {
        DrawGazeRayAndPosition(leftEyePos,leftEyeDir, Color.red, Color.blue);

        DrawGazeRayAndPosition(rightEyePos,rightEyeDir, Color.green, Color.blue);
    }

    private void DrawGazeRayAndPosition(Vector3 eyePos,Vector3 eyeDirection, Color rayColor, Color hitColor)
    {
        Vector3 gazePosition;
        RaycastHit hit;

        Ray gazeRay = new Ray(eyePos, eyeDirection);

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
        Gizmos.DrawLine(eyePos, gazePosition);

        // Draw a sphere at the gaze position
        Gizmos.color = hitColor;
        Gizmos.DrawWireSphere(gazePosition, 0.1f);

        var screenPosition = mainCamera.WorldToScreenPoint(gazePosition);
        //Debug.Log($"Eye pos: {eyePos}, Eye direction: {eyeDirection}, Screen Position: {screenPosition}"); //Used for debugging
    }
}
