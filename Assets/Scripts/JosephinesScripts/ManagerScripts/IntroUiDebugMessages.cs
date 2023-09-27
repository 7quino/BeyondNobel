using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.XR.ARFoundation;
using System;

public class IntroUiDebugMessages : MonoBehaviour
{
    public static IntroUiDebugMessages Instance;

    public ARSession Session;
    public AREarthManager EarthManager;

    public GameObject InfoPanel;
    public TextMeshProUGUI DebugTextLocation;
    public TextMeshProUGUI DebugText;
    public TextMeshProUGUI InfoText;
    public TextMeshProUGUI SnackBarText;

    private bool _privancyPromptOk = false;

    private void Awake()
    {
        Instance = this;
    }

    public void OnEnable()
    {
        InfoPanel.SetActive(false);
    }

    private void Start()
    {
        CheckARSession.Instance.OnArSessionStopped.AddListener(OnArSessionStopped);
        UiManager.instance.onPrivacyPromptIsOk.AddListener((userInput) => { _privancyPromptOk = userInput; });
        CheckLocationService.Instance.onDebugMessage.AddListener((message) => { SnackBarText.text = message; });
        ScreenSettingsManager.Instance.onDebugMessage.AddListener((message) => { DebugText.text = message; });
        UiManager.instance.onDebugMessage.AddListener((message) => { DebugText.text += "\n" + message; });
    }
    
    public void Update()
    {
        if (!_privancyPromptOk) return;

        UpdateDebugInfo();

        if (CheckARSession.Instance.isReturning) return;
        if (ARSession.state != ARSessionState.SessionInitializing && ARSession.state != ARSessionState.SessionTracking) return;

        var earthTrackingState = EarthManager.EarthTrackingState;
        var pose = earthTrackingState == TrackingState.Tracking ? EarthManager.CameraGeospatialPose : new GeospatialPose();
        if (!InfoPanel.activeSelf) InfoPanel.SetActive(true);
        if (earthTrackingState == TrackingState.Tracking)
        {
            InfoText.text = string.Format(
            "Latitude/Longitude: {1}°, {2}°{0}" +
            "Horizontal Accuracy: {3}m{0}" +
            "Altitude: {4}m{0}" +
            "Vertical Accuracy: {5}m{0}" +
            "Eun Rotation: {6}{0}" +
            "Orientation Yaw Accuracy: {7}°",
            Environment.NewLine,
            pose.Latitude.ToString("F6"),
            pose.Longitude.ToString("F6"),
            pose.HorizontalAccuracy.ToString("F6"),
            pose.Altitude.ToString("F2"),
            pose.VerticalAccuracy.ToString("F2"),
            pose.EunRotation.ToString("F1"),
            pose.OrientationYawAccuracy.ToString("F1"));
        }
        else
        {
            InfoText.text = "GEOSPATIAL POSE: not tracking";
        }
    }

    void UpdateDebugInfo()
    {
        if (EarthManager == null) return;
        var pose = EarthManager.EarthState == EarthState.Enabled &&
        EarthManager.EarthTrackingState == TrackingState.Tracking ?
            EarthManager.CameraGeospatialPose : new GeospatialPose();
        var supported = EarthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
        DebugTextLocation.text =
            $"IsReturning: {CheckARSession.Instance.isReturning}\n" +
            $"IsLocalizing: {CheckLocationService.Instance.isLocalizing}\n" +
            $"SessionState: {ARSession.state}\n" +
            $"LocationServiceStatus: {Input.location.status}\n" +
            $"FeatureSupported: {supported}\n" +
        $"EarthState: {EarthManager.EarthState}\n" +
            $"EarthTrackingState: {EarthManager.EarthTrackingState}\n" +
            $"  LAT/LNG: {pose.Latitude:F6}, {pose.Longitude:F6}\n" +
            $"  HorizontalAcc: {pose.HorizontalAccuracy:F6}\n" +
            $"  ALT: {pose.Altitude:F2}\n" +
            $"  VerticalAcc: {pose.VerticalAccuracy:F2}\n" +
            $". EunRotation: {pose.EunRotation:F2}\n" +
            $"  OrientationYawAcc: {pose.OrientationYawAccuracy:F2}";
    }

    void OnArSessionStopped(string reason)
    {
        InfoPanel.SetActive(false);
        SnackBarText.text = reason;
    }
}
