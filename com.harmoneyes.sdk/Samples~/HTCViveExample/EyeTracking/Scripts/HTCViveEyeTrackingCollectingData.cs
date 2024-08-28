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
        if (EyeManager.Instance != null) { EyeManager.Instance.EnableEyeTracking = true; }

        Debug.Log($"Eye Tracking status: {EyeManager.Instance.GetEyeTrackingStatus()}");
        EyeManager.Instance.GetLeftEyeOrigin(out leftEyePos);
        EyeManager.Instance.GetRightEyeOrigin(out rightEyePos);

        EyeManager.Instance.GetLeftEyeDirectionNormalized(out leftEyeDir);
        EyeManager.Instance.GetRightEyeDirectionNormalized(out rightEyeDir);

        EyeManager.Instance.GetLeftEyePupilDiameter(out leftPupilDiameter);
        EyeManager.Instance.GetRightEyePupilDiameter(out rightPupilDiameter);

        EyeManager.Instance.GetLeftEyeOpenness(out eyeOpenLeft);
        EyeManager.Instance.GetRightEyeOpenness(out eyeOpenRight);

        EyeTrackingConfig.Instance.SessionTime += Time.deltaTime;
        double timeStamp = EyeTrackingConfig.Instance.SessionTime;

        var eyeSample = EyeTrackingConfig.Instance.EyeTrackingAnalyzer.CreateSample(timeStamp, leftEyePos, leftEyeDir, eyeOpenLeft, leftPupilDiameter, rightEyePos, rightEyeDir, eyeOpenRight, rightPupilDiameter, mainCamera);

        //Debug.Log($"[HarmonEyes SDK] Left Eye Origin: {eyeSample.LeftEye.EyeOrigin.X}, {eyeSample.LeftEye.EyeOrigin.Y}, {eyeSample.LeftEye.EyeOrigin.Z}");
        //Debug.Log($"[HarmonEyes SDK] Left Eye Direction: {eyeSample.LeftEye.EyeDirection.X}, {eyeSample.LeftEye.EyeDirection.Y}, {eyeSample.LeftEye.EyeDirection.Z}");
        //Debug.Log($"[HarmonEyes SDK] Left Eye Gaze: {eyeSample.LeftEye.GazeX}, {eyeSample.LeftEye.GazeY}");

        //Debug.Log($"[HarmonEyes SDK] Eye Open: {eyeSample.LeftEye.EyeOpenness}, {eyeSample.RightEye.EyeOpenness}");

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
