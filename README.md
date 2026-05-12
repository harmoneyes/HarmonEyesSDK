# HarmonEyes Unity SDK — Overview & Getting Started

## Overview

Find instructions here on installing the unity sdk: [Unity SDK Install](https://harmoneyes.com/developers/unity-sdk/unity-sdk-install/).

The HarmonEyes Unity SDK turns raw eye-tracking data from a supported headset or webcam into four real-time cognitive-state metrics: **Mental Workload**, **Fatigue**, **Attention**, and **Mental Readiness**. The SDK is delivered as a Unity package that ships a native theia library, a C# wrapper, three platform sample scenes (Meta Quest Pro, HTC Vive Focus Vision, and a headset-free Mock playback), and editor scripts for inspector ergonomics.

## What it produces

| Metric | Levels | Cadence | Key payload fields |
|---|---|---|---|
| Mental Workload | Low / Moderate / High | per native batch (~1 s) | `level`, `confidence`, `batch_number` |
| Fatigue | Alert / Neither / RatherDrowsy / Drowsy | per native batch (~1 s) | `level`, `confidence`, `batch_number` |
| Attention | Narrow / Mod-Narrow / Neither / Mod-Broad / Broad | continuous, advances with fixation/saccade observations | `level`, `label`, `composite_score`, `total_observations` |
| Mental Readiness | low % / moderate % / high % | once minimum session length is met | `low_percentage`, `moderate_percentage`, `high_percentage`, `total_predictions` |

See the [API Reference](https://harmoneyes.com/developers/unity-sdk/documentation/api-reference/) for the per-field type and range.

## Real-world use cases

- **Reading performance** — line-by-line attention tracking, comprehension monitoring, detecting reading-difficulty patterns.
- **Clinical diagnostics** — screening for Parkinson's, TBI, Lyme disease, MCI, MS via eye-movement biomarkers.
- **Visual search and driving** — monitoring attention, decision-making, situational awareness.
- **Biometric ID** — eye-movement signatures for real-time user authentication.
- **Developer testing** — user testing during app development.
- **Market research** — interface usability and user-behavior insight.

## Supported hardware

| Tracker | Native preset | Sample scene |
|---|---|---|
| Meta Quest Pro | Quest (72 Hz) | [Meta Quest Pro Sample](https://harmoneyes.com/developers/unity-sdk/meta-quest-pro-sample/) |
| HTC Vive Focus Vision | Quest preset (closest available match) | [HTC Vive Focus Vision Sample](https://harmoneyes.com/developers/unity-sdk/htc-vive-focus-sample/) |
| Pupil Labs Neon | PupilLabs (200 Hz) | Mock Tracker Sample (CSV playback) |
| Webcam-based desktop trackers | Webcam (30 Hz) | — |
| Ganzin | Ganzin (60 Hz) | — |

*To set up any eye tracker not listed, select the nearest lower Hz preset from this list.*

## Get a license key

A license key is required. Contact [sales@harmoneyes.com](mailto:sales@harmoneyes.com) to obtain one. The same key is entered on the `AnalyzeEyeTrackingData` component in each sample scene.

## Three ways to use the SDK

Pick whichever matches your situation.

| Path | When to use | See |
|---|---|---|
| Headset sample | You have a Meta Quest Pro or Vive Focus Vision and want a working scene out of the box. | [Meta Quest Pro Sample](https://harmoneyes.com/developers/unity-sdk/meta-quest-pro-sample/) / [HTC Vive Focus Vision Sample](https://harmoneyes.com/developers/unity-sdk/htc-vive-focus-sample/) |
| Headset-free Mock playback | You don't have a headset, or you want to test against a recorded CSV. | [Unity SDK Install](https://harmoneyes.com/developers/unity-sdk/unity-sdk-install/) → Mock Tracker section |
| Write a gaze data collection and submission script for your tracker | Your platform isn't covered by the bundled samples or has a more complex setup. | Manual Integration Overview (below) / [API Reference](https://harmoneyes.com/developers/unity-sdk/documentation/api-reference/) |

## Manual Integration Overview

A custom integration is three steps.

### 1. Initialise

- Add the `AnalyzeEyeTrackingData` component to a GameObject in your scene.
- Enter the License Key on the component inspector.
- Pick the Tracker preset that matches your hardware (or the closest sample-rate match, rounding down).

The component handles native SDK creation, license validation, and session start automatically on `Start`. Read `AnalyzeEyeTrackingData.Instance.EyeTrackingInitCompleted` to know when initialisation has completed.

### 2. Capture — submit one sample per tracker frame

Write a collector MonoBehaviour that reads your platform's eye data, builds a `Sample` via the factory, and submits it.

```csharp
using UnityEngine;
using HarmonEyes.EyeTracking.Common;

public class MyCollector : MonoBehaviour
{
    [SerializeField] private Camera headCamera;

    void FixedUpdate()
    {
        var analyze = AnalyzeEyeTrackingData.Instance;
        if (analyze == null || !analyze.EyeTrackingInitCompleted) return;

        long timestampNs = (long)(Time.timeAsDouble * 1e9);

        // Pull per-eye origin, orientation, and a blink decision
        // from your platform's API.
        Vector3    leftEyeOrigin       = /* head-local metres */ Vector3.zero;
        Quaternion leftEyeOrientation  = /* gaze rotation */ Quaternion.identity;
        Vector3    rightEyeOrigin      = Vector3.zero;
        Quaternion rightEyeOrientation = Quaternion.identity;
        bool       isBlinking          = /* your platform's blink criterion */ false;

        Sample s = GazeDataSampleFactory.CreateGazeSample(
            analyze.SessionId, timestampNs,
            leftEyeOrigin,  leftEyeOrientation,
            rightEyeOrigin, rightEyeOrientation,
            headCamera.fieldOfView,
            isBlinking);

        analyze.SubmitGazeSample(s);
    }
}
```

### 3. Process — read results

Two access patterns. Pick whichever fits your code.

**Polling** — read the latest payload on demand:

```csharp
var mw = analyze.EyeTrackingAnalyzer.CurrentMentalWorkload;
if (mw != null) Debug.Log($"MW: {mw.LevelName} (conf {mw.confidence:F2})");
```

**Event-driven** — subscribe once, react to each new prediction:

```csharp
analyze.EyeTrackingAnalyzer.OnMentalWorkloadResult += mw =>
    Debug.Log($"MW: {mw.LevelName} (conf {mw.confidence:F2})");
```

The four result properties and matching events are listed in the [API Reference](https://harmoneyes.com/developers/unity-sdk/documentation/api-reference/).

## Where to go next

| Goal | Link |
|---|---|
| Install the SDK and run the headset-free Mock sample | [Unity SDK Install](https://harmoneyes.com/developers/unity-sdk/unity-sdk-install/) |
| Run the Meta Quest Pro sample | [Meta Quest Pro Sample](https://harmoneyes.com/developers/unity-sdk/meta-quest-pro-sample/) |
| Run the Vive Focus Vision sample | [HTC Vive Focus Vision Sample](https://harmoneyes.com/developers/unity-sdk/htc-vive-focus-sample/) |
| Full public-API reference (components, factory, results, events, native wrappers) | [API Reference](https://harmoneyes.com/developers/unity-sdk/documentation/api-reference/) |
| Watch the Unity SDK demo video | [Unity SDK Demo Video](https://harmoneyes.com/developers/unity-sdk/unity-sdk-demo-video/) |
