# HarmonEyes SDK

## Analysis Outputs

The SDK provides three real-time metrics, updated each second:

- Cognitive Load — mental demand and processing effort during a task
- Fatigue — physical and cognitive tiredness affecting alertness and performance
- Motion Sickness — discomfort caused by sensory mismatch between visual motion and physical movement

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
2. In the inspector fields in EyeTrackingConfig, enter your license key and select your device type in the EyeTrackingConfig Inspector.

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

		// Create a timestamp relative to session start
		long timeStamp = (long)(Time.timeAsDouble - EyeTrackingConfig.Instance.SessionStartTime)

		// Build a sample from your platform's eye tracking data
		Sample eyeSample = EyeTrackingConfig.Instance.EyeTrackingAnalyzer.CreateSample(
			timeStamp,
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

	EyeTrackingConfig.Instance.currentCogLoad
	EyeTrackingConfig.Instance.currentFatigue
	EyeTrackingConfig.Instance.currentMotionSickness


Each metric reports a current level (Low, Mod, High) and transition predictions for level changes. Transitions always predict upward — `TransitionOne` predicts the next level up from the current level, and `TransitionTwo` predicts the level after that. All three metrics share the same property structure:


	// Current level
	.CurrentLevel.Value           // "Low"
	.CurrentLevel.Message         // "Fatigue Level Now: Low"
	.CurrentLevel.HumanReadable   // "Fatigue Level Now: Low, Current fatigue level."

	// Current probability — probability of being at the current level
	.CurrentProbability.Value          // 100
	.CurrentProbability.Message        // "Fatigue Prob Now: 100%"
	.CurrentProbability.HumanReadable  // "Fatigue Prob Now: 100%, Probability at current fatigue level."

	// Predicted transition time — milliseconds until the next level up
	.TransitionOne.TransitionTime.Value          // 5543.787
	.TransitionOne.TransitionTime.Message        // "Fatigue Predict Mod When: 15h 23m 57s (55437868 milliseconds)"
	.TransitionOne.TransitionTime.HumanReadable  // "Fatigue Predict Mod When: 15h 23m 57s (55437868 milliseconds), Time to Mod level. Note: this assumes you keep engaged with the activity as you are now."

	// Lower bound — earliest expected transition (RMSE minus 1 standard deviation)
	.TransitionOne.TransitionLowerBound.Value          // 4860.937
	.TransitionOne.TransitionLowerBound.Message        // "Fatigue Predict High Lower Bound: 0m 4s (4860 milliseconds)"
	.TransitionOne.TransitionLowerBound.HumanReadable  // "Fatigue Predict High Lower Bound: 0m 4s (4860 milliseconds}, The low (floor) for the change to High fatigue. This is calculated based on the average root mean square error (rsme) minus 1 standard deviation (-1SD)."

	// Upper bound — latest expected transition (RMSE plus 1 standard deviation)
	.TransitionOne.TransitionUpperBound.Value          // 5941.083
	.TransitionOne.TransitionUpperBound.Message        // "Fatigue Predict High Upper Bound: 0m 5s (5941 milliseconds)"
	.TransitionOne.TransitionUpperBound.HumanReadable  // "Fatigue Predict High Upper Bound: 0m 5s (5941 milliseconds}, The high (ceiling) for the change to High fatigue. This is calculated based on the average root mean square error (rsme) plus 1 standard deviation (+1SD)."


`TransitionTwo` has the same structure as `TransitionOne` but predicts the level after that (e.g. if current level is Low, `TransitionOne` predicts Mod, `TransitionTwo` predicts High). At Mod, both `TransitionOne` and `TransitionTwo` predict High since there is no level above it.


## Meta Quest Sample

A ready-to-use Meta Quest sample is included as a .unitypackage. Double-click MetaXRSample.unitypackage in the Samples folder to open the Import dialog, then click Import. Open HarmonEyesDemoOculusScene and enter your license key on the EyeTrackingDataOculusAnalyzer prefab. Headset type is preconfigured (Quest Pro).

See `MetaXRSampleInstructions.md` in the Samples folder for full details, including:

- Required dependencies
- Step-by-step setup instructions
- How to verify eye tracking and SDK output are working correctly

## Requirements

Minimum Unity version: 2022.3 LTS or newer

### Core SDK Dependencies

Install via Package Manager (Unity Registry):

- TextMeshPro — 3.0.7 or newer
  https://docs.unity3d.com/Packages/com.unity.textmeshpro@latest/

- Newtonsoft JSON — 3.2.1 or newer
  https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@latest/

### Meta XR Sample Dependencies

- Set you your Unity Project for the Vive Focus Vision with the manufacturer instructions here:
https://developer.vive.com/resources/openxr/unity/tutorials/setup-and-installation/getting-started-with-openxr/

- For full eye tracking, you'll need to enable the eye and face tracking components in Projects Settings:
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
