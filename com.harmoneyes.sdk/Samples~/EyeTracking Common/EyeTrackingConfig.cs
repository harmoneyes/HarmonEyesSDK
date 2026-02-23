using HarmonEyesSDK.EyeTrackingPlugin;
using HarmonEyesSDK.OnnxSessions;
using UnityEngine;
using HarmonEyesScreen = HarmonEyesSDK.DataModels.EyeTrackerDataModels.Screen;

public class EyeTrackingConfig : MonoBehaviour {
    public static EyeTrackingConfig Instance;
    public double SessionStartTime;
    public double SessionTime;
    public EyeTrackingData EyeTrackingData;
    public bool EyeTrackingInitCompleted;
    public EyeTrackingAnalyzer EyeTrackingAnalyzer;

    [SerializeField] private string LicenseKey;

    [SerializeField] private HarmonEyesSDK.EyeTrackingPlugin.Enums.EyeTrackingEnums.DeviceType DeviceType;

    [Header("Report Cognitive Load")] public bool reportCognitiveLoad = true;

    [Header("Cognitive Load Reporting Frequency (seconds)")] [Range(1, 10)]
    public int cognitiveLoadReportingFrequency;

    [Header("Cognitive Load Alert To High (seconds)")] [Range(1, 60)]
    public int cognitiveLoadAlertToHigh;

    [Header("Report Fatigue")] public bool reportFatigue = true;

    [Header("Fatigue Reporting Frequency (seconds)")] [Range(1, 10)]
    public int fatigueReportingFrequency;

    [Header("Fatigue Alert To High (seconds)")] [Range(1, 60)]
    public int fatigueAlertToHigh;

    [Header("Report Motion Sickness")] public bool reportMotionSickness = true;

    [Header("Motion Sickness Reporting Frequency (seconds)")]
    [Range(1, 10)]
    public int motionSicknessReportingFrequency;

    [Header("Motion Sickness Alert To High (seconds)")]
    [Range(1, 60)]
    public int motionSicknessAlertToHigh;

    private float cogLoadTimer = 0.0f;
    private float fatigueTimer = 0.0f;
    private float motionSicknessTimer = 0.0f;
    private bool initialized = false;
    public CognitiveLoadData currentCogLoad;
    public FatigueData currentFatigue;
    public MotionSicknessData currentMotionSickness;
   

    #region Singleton

    private void Awake() {
        Instance = this;
        Initialize();
    }

    #endregion

    private void Start() {
        initialized = true;
    }

    public void Initialize() {
        EyeTrackingAnalyzer = new EyeTrackingAnalyzer();

        SessionStartTime = Time.timeAsDouble;
        SessionTime = SessionStartTime;

        EyeTrackingData = new EyeTrackingData();

        HarmonEyesScreen screen = new HarmonEyesScreen(Screen.width, Screen.height, Screen.dpi);

        EyeTrackingAnalyzer.Initialize(screen, DeviceType, LicenseKey);
        EyeTrackingInitCompleted = true;

        cognitiveLoadReportingFrequency = 1;
        fatigueReportingFrequency = 1;
        motionSicknessReportingFrequency = 1;
    }

    void Update() {
        cogLoadTimer += Time.deltaTime;
        fatigueTimer += Time.deltaTime;
        motionSicknessTimer += Time.deltaTime;

        if (initialized && Instance.EyeTrackingInitCompleted) {
            if (reportCognitiveLoad && cogLoadTimer > cognitiveLoadReportingFrequency && currentCogLoad != null) {
                Debug.Log("[HarmonEyes SDK] " + "Cog Load Level: " + currentCogLoad.CurrentLevel.Value);
                Debug.Log("[HarmonEyes SDK] " + currentCogLoad.TransitionOne.TransitionTime.HumanReadable);
                Debug.Log("[HarmonEyes SDK] " + currentCogLoad.TransitionOne.TransitionLowerBound.HumanReadable);
                Debug.Log("[HarmonEyes SDK] " + currentCogLoad.TransitionOne.TransitionUpperBound.HumanReadable);
                
                cogLoadTimer = 0;
            }

            if (reportFatigue && fatigueTimer > fatigueReportingFrequency && currentFatigue != null) {
                Debug.Log("[HarmonEyes SDK] " + "Fatigue Level: " + currentFatigue.CurrentLevel.Value);
                Debug.Log("[HarmonEyes SDK] " + currentFatigue.TransitionOne.TransitionTime.HumanReadable);
                Debug.Log("[HarmonEyes SDK] " + currentFatigue.TransitionOne.TransitionLowerBound.HumanReadable);
                Debug.Log("[HarmonEyes SDK] " + currentFatigue.TransitionOne.TransitionUpperBound.HumanReadable);

                fatigueTimer = 0;
            }

            if (reportMotionSickness && motionSicknessTimer > motionSicknessReportingFrequency && currentMotionSickness != null)
            {
                Debug.Log("[HarmonEyes SDK] " + "Motion Sickness Level: " + currentMotionSickness.CurrentLevel.Value);
                Debug.Log("[HarmonEyes SDK] " + currentMotionSickness.TransitionOne.TransitionTime.HumanReadable);
                Debug.Log("[HarmonEyes SDK] " + currentMotionSickness.TransitionOne.TransitionLowerBound.HumanReadable);
                Debug.Log("[HarmonEyes SDK] " + currentMotionSickness.TransitionOne.TransitionUpperBound.HumanReadable);

                motionSicknessTimer = 0;
            }
        }
    }
}
