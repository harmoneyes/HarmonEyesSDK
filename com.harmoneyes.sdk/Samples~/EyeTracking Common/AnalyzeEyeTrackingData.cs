using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using HarmonEyesSDK.OnnxSessions;

public class AnalyzeEyeTrackingData : MonoBehaviour
{
    private double lastSent;
    private CognitiveLoadData currCogLoad;
    private FatigueData currFatigue;
    private MotionSicknessData currMotionSickness;

    [SerializeField]
    private TextMeshPro EyeTrackingResponseText;

    void Start()
    {
        lastSent = Time.timeAsDouble; 
    }

    // Update is called once per frame
    void Update()
    {
        var testDuration = Time.timeAsDouble - EyeTrackingConfig.Instance.SessionStartTime;
        if (Time.timeAsDouble - lastSent >= 1f && EyeTrackingConfig.Instance.EyeTrackingInitCompleted)
        {
            lastSent = Time.timeAsDouble;
            AnalyzeEyeTracking(testDuration);
        }
    }

    private void AnalyzeEyeTracking(double testDuration)
    {
        EyeTrackingConfig.Instance.EyeTrackingData.TotalDuration = testDuration;
        if (EyeTrackingConfig.Instance.EyeTrackingData.Samples.GetSamples().Count > 0)
        {
            currCogLoad = EyeTrackingConfig.Instance.EyeTrackingAnalyzer.GetCurrentCogLoad(EyeTrackingConfig.Instance.EyeTrackingData);
            currFatigue = EyeTrackingConfig.Instance.EyeTrackingAnalyzer.GetCurrentFatigue(EyeTrackingConfig.Instance.EyeTrackingData);
            currMotionSickness = EyeTrackingConfig.Instance.EyeTrackingAnalyzer.GetCurrentMotionSickness(EyeTrackingConfig.Instance.EyeTrackingData);

            EyeTrackingConfig.Instance.currentCogLoad = currCogLoad;
            EyeTrackingConfig.Instance.currentFatigue = currFatigue;
            EyeTrackingConfig.Instance.currentMotionSickness = currMotionSickness;

            //Used for displaying input & response info
            StartCoroutine(DisplayEyeTrackingResponse());

            // Clear Eye Tracking Data
            EyeTrackingConfig.Instance.EyeTrackingData.ClearData();
        }
    }

    IEnumerator DisplayEyeTrackingResponse()
    {
        EyeTrackingResponseText.text = "Current Fatigue Level: " + currFatigue.CurrentLevel.Value + "\n "+ currFatigue.TransitionOne.TransitionTime.HumanReadable
                                        + "\n " + currFatigue.TransitionOne.TransitionLowerBound.HumanReadable
                                        + "\n " + currFatigue.TransitionOne.TransitionUpperBound.HumanReadable;
        
        // Wait for 0.1 seconds
        yield return new WaitForSeconds(0.1f);
    }

}
