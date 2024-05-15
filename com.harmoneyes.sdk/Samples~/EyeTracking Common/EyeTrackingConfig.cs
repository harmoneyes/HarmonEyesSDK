using HarmonEyesSDK.EyeTrackingPlugin;
using HarmonEyesSDK.OnnxSessions;
using UnityEngine;
using HarmonEyesScreen = HarmonEyesSDK.DataModels.EyeTrackerDataModels.Screen;

public class EyeTrackingConfig : MonoBehaviour {
    public static EyeTrackingConfig Instance;
    public double SessionStartTime;
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

    private float cogLoadTimer = 0.0f;
    private float fatigueTimer = 0.0f;
    private bool initialized = false;
    public CognitiveLoadData currentCogLoad;
    public FatigueData currentFatigue;

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

        EyeTrackingData = new EyeTrackingData();

        HarmonEyesScreen screen = new HarmonEyesScreen(Screen.width, Screen.height, Screen.dpi);

        EyeTrackingAnalyzer.Initialize(screen, DeviceType, LicenseKey);
        EyeTrackingInitCompleted = true;
    }

    void Update() {
        cogLoadTimer += Time.deltaTime;
        fatigueTimer += Time.deltaTime;

        if (initialized && Instance.EyeTrackingInitCompleted) {
            if (reportCognitiveLoad && cogLoadTimer > cognitiveLoadReportingFrequency && currentCogLoad != null) {
                Debug.Log("[HarmonEyes SDK] " + " Cog Load Level: " + currentCogLoad.CurrentLevel.Value);
                Debug.Log("[HarmonEyes SDK] " + currentCogLoad.TransitionOne.TransitionTime.HumanReadable);
                Debug.Log("[HarmonEyes SDK] " + currentCogLoad.TransitionOne.TransitionLowerBound.HumanReadable);
                Debug.Log("[HarmonEyes SDK] " + currentCogLoad.TransitionOne.TransitionUpperBound.HumanReadable);

                cogLoadTimer -= cognitiveLoadReportingFrequency;
            }

            if (reportFatigue && fatigueTimer > fatigueReportingFrequency && currentFatigue != null) {
                Debug.Log("[HarmonEyes SDK] " + "Fatigue Level: " + currentFatigue.CurrentLevel.Value);
                Debug.Log("[HarmonEyes SDK] " + currentFatigue.TransitionOne.TransitionTime.HumanReadable);
                Debug.Log("[HarmonEyes SDK] " + currentFatigue.TransitionOne.TransitionLowerBound.HumanReadable);
                Debug.Log("[HarmonEyes SDK] " + currentFatigue.TransitionOne.TransitionUpperBound.HumanReadable);

                cogLoadTimer -= cognitiveLoadReportingFrequency;
            }
        }
    }
}
