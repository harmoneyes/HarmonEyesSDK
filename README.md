# HarmonEyes SDK

## Analysis Outputs

The SDK provides two real-time metrics, updated each second:

- Cognitive Load — mental demand and processing effort during a task
- Drowsiness / Sleepiness — alertness level, with an estimate of minutes until transitioning to the next state

## Applications

- Reading performance — real-time line-by-line tracking, comprehension improvement, detecting reading issues
- Clinical diagnostics — screening for Parkinson's, TBI, Lyme disease, MCI, MS via eye movement biomarkers
- Visual search / driving — monitoring attention, decision-making, situational awareness
- Biometric ID — eye movements as real-time user authentication
- Developer testing — user testing during app development
- Market research — interface usability and user behavior insights

## Getting a License

A license key is required to use the HarmonEyes SDK. Contact sales@harmoneyes.com to obtain one.

## Quick Start

1. Add `EyeTrackingConfig` and `AnalyzeEyeTrackingData` components to a GameObject in your scene.
2. In the inspector fields in EyeTrackingConfig, enter your license key.

The SDK validates your license key on startup. If the key is missing or invalid, initialization stops and an error is logged to the console. Check `LicenseKeyValidated` and `LicenseKeyError` at runtime to confirm status in your own code:

```csharp
if (EyeTrackingConfig.Instance.LicenseKeyValidated) ...
if (EyeTrackingConfig.Instance.LicenseKeyError) ...
```

## Submitting Eye Tracking Data

To integrate the HarmonEyes SDK, your will collect eye data from your eye tracking enabled hardware and submit samples. Samples can be submitted each frame or based on the sample rate of your hardware. Each sample requires a Transform for each eye (gaze origin and direction), a float for each eye's openness (0.0 = open, 1.0 = closed), and a reference to a camera tied to the user’s ‘center eye’ position (typically center eye camera object exists in XR rigs). 


	void Update()
	{
		if (!EyeTrackingConfig.Instance.LicenseKeyValidated)
			return;

		// Create a nanosecond timestamp relative to session start
		long timestampNs = (long)((Time.timeAsDouble - EyeTrackingConfig.Instance.SessionStartTime) * 1e9);

		// Build a sample from your platform's eye tracking data
		Sample eyeSample = GazeDataFormatter.BuildSampleFromEyeTransforms(
			timestampNs,
			leftEyeTransform,   // Transform — left eye gaze origin and direction
			leftEyeClosed,       // float — 0.0 open, 1.0 closed
			rightEyeTransform,  // Transform — right eye gaze origin and direction
			rightEyeClosed,      // float — 0.0 open, 1.0 closed
			centerEyeCamera           // Camera — the player's primary camera (e.g. center eye camera in VR)
		);

		// Submit the sample to the SDK
		EyeTrackingConfig.Instance.EyeTrackingData.Samples.AddSample(eyeSample);
	}


The `AnalyzeEyeTrackingData` component automatically processes accumulated samples each second and stores the results on `EyeTrackingConfig.Instance`.

## Accessing Results

Results are available from any script via the `EyeTrackingConfig` singleton, as long as both `EyeTrackingConfig` and `AnalyzeEyeTrackingData` components are in the scene:

	EyeTrackingConfig.Instance.currentCogLoad     // CognitiveLoadData, nullable
	EyeTrackingConfig.Instance.currentSleepiness  // SleepinessData,    nullable


Each payload is a flat data object. Both fields start `null` and populate only when the native SDK emits its first prediction for that metric — null-check before use. The `batch_number` field increases monotonically per completed analysis window; consumers can edge-trigger UI updates when it advances.


	// Cognitive load — CognitiveLoadData
	.level         // int    — 0 = Low, 1 = Moderate, 2 = High
	.confidence    // float  — 0..1
	.prob_high     // float  — probability the user is in the High state
	.batch_number  // int    — window index
	.LevelName     // string — "Low" | "Moderate" | "High"

	// Drowsiness / sleepiness — SleepinessData
	.level         // int    — 0 = Alert, 1 = Neither, 2 = RatherDrowsy, 3 = Drowsy
	.confidence    // float  — 0..1
	.batch_number  // int    — window index
	.ttt_level     // int    — transition-target level (same enum as .level)
	.ttt_minutes   // int    — estimated minutes until transitioning to ttt_level
	.LevelName     // string — "Alert" | "Neither" | "RatherDrowsy" | "Drowsy"


## Meta Quest Sample

A ready-to-use Meta Quest sample is included as a .unitypackage. Double-click MetaXRSample.unitypackage in the Samples folder to open the Import dialog, then click Import. Open HarmonEyesDemoOculusScene and enter your license key on the EyeTrackingDataOculusAnalyzer prefab. Headset type is preconfigured (Quest Pro).

See `MetaXRSampleInstructions.md` in the Samples folder for full details, including:

- Required dependencies
- Step-by-step setup instructions
- How to verify eye tracking and SDK output are working correctly

## VIVE Focus Vision Sample

A ready-to-use VIVE Focus Vision sample is included as a .unitypackage. Double-click ViveSample.unitypackage in the Samples folder to open the Import dialog, then click Import. Open HarmonEyesDemoViveScene and enter your license key on the EyeTrackingDataViveAnalyzer prefab. Headset type is preconfigured (VIVE Focus Vision).

See `ViveFocusVisionSampleInstructions.md` in the Samples folder for full details, including:

- Required dependencies
- Step-by-step setup instructions (including enabling `VIVE XR Eye Tracking (Beta)` and `Facial Tracking` under OpenXR project settings)
- How to verify eye tracking and SDK output are working correctly

## Requirements

Minimum Unity version: 2022.3 LTS or newer

### Core SDK Dependencies

Install via Package Manager (Unity Registry):

- TextMeshPro — 3.0.7 or newer
  https://docs.unity3d.com/Packages/com.unity.textmeshpro@latest/

### VIVE Focus Vision Sample Dependencies

- OpenXR Plugin — 1.14.3 or newer (Unity Registry)
  https://docs.unity3d.com/Packages/com.unity.xr.openxr@latest/

- Set up your Unity project for the VIVE Focus Vision using the manufacturer instructions here:
https://developer.vive.com/resources/openxr/unity/tutorials/setup-and-installation/getting-started-with-openxr/

- For full eye tracking, you'll need to enable the eye and face tracking components in Project Settings:
  In Unity, click Edit > Project Settings > XR Plug-in Management > OpenXR > and check the boxes for VIVE XR Eye Tracking (Beta) and Facial Tracking

### Meta XR Sample Dependencies

- OpenXR Plugin — 1.14.3 or newer (Unity Registry)
  https://docs.unity3d.com/Packages/com.unity.xr.openxr@latest/

- Meta XR Core SDK — 85.0.0 or newer
  https://assetstore.unity.com/packages/tools/integration/meta-xr-core-sdk-269169

- Meta XR Interaction OVR — 85.0.0 or newer
  https://assetstore.unity.com/packages/tools/integration/meta-xr-interaction-sdk-265014

## Third-Party Licenses

This package includes Microsoft ONNX Runtime (MIT License).
See Third-Party Notices.txt included in the package for full details.
