using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CheckLocationService : MonoBehaviour
{
    public static CheckLocationService Instance;
    public UnityEvent onLocationServiceSuccess = new UnityEvent();
    public UnityEvent onLocationServiceError = new UnityEvent();
    public UnityEvent<string> onDebugMessage = new UnityEvent<string>();

    public AREarthManager EarthManager;
    public ARCoreExtensions ARCoreExtensions;

    [HideInInspector] public bool waitingForLocationService = false;
    [HideInInspector] public bool isLocalizing = false;
    private float _configurePrepareTime = 3f;
    private IEnumerator _startLocationService = null;
    private float _localizationPassedTime = 0f;
    private bool _enablingGeospatial = false;
    private bool _privancyPromptOk = false;
    private bool _localisationServiceOk = false;

    private const string _localizingMessage = "Localizing your device to set anchor.";
    private const string _localizationInitializingMessage = "Initializing Geospatial functionalities.";
    private const string _localizationInstructionMessage = "Point your camera at buildings, stores, and signs near you.";
    private const string _localizationFailureMessage = "Localization not possible.\n" + "Close and open the app to restart the session.";
    private const string _localizationSuccessMessage = "Localization completed.";
    private const double _orientationYawAccuracyThreshold = 25;
    private const double _horizontalAccuracyThreshold = 20;
    private const float _timeoutSeconds = 180;

    private void Awake()
    {
        Instance = this;
    }

    public void OnEnable()
    {
        _startLocationService = StartLocationService();
        StartCoroutine(_startLocationService);

        _localizationPassedTime = 0f;
        isLocalizing = true;
        onDebugMessage.Invoke(_localizingMessage);

        _enablingGeospatial = false;
    }

    private void Start()
    {
        UiManager.instance.onPrivacyPromptIsOk.AddListener((userInput) => { _privancyPromptOk = userInput; });
    }

    public void Update()
    {
        if (!_privancyPromptOk) return;
        if (CheckARSession.Instance.isReturning) return;
        if (ARSession.state != ARSessionState.SessionInitializing && ARSession.state != ARSessionState.SessionTracking) return;
        if (_localisationServiceOk) return;

        // Check feature support and enable Geospatial API when it's supported.
        var featureSupport = EarthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
        switch (featureSupport)
        {
            case FeatureSupported.Unknown:
                return;
            case FeatureSupported.Unsupported:
                CheckARSession.Instance.ReturnWithReason("The Geospatial API is not supported by this device.");
                onLocationServiceError.Invoke();
                return;
            case FeatureSupported.Supported:
                if (ARCoreExtensions.ARCoreExtensionsConfig.GeospatialMode ==
                    GeospatialMode.Disabled)
                {
                    onDebugMessage.Invoke("Geospatial sample switched to GeospatialMode.Enabled.");
                    ARCoreExtensions.ARCoreExtensionsConfig.GeospatialMode =
                        GeospatialMode.Enabled;
                    ARCoreExtensions.ARCoreExtensionsConfig.StreetscapeGeometryMode =
                        StreetscapeGeometryMode.Enabled;
                    _configurePrepareTime = 3.0f;
                    _enablingGeospatial = true;
                    return;
                }

                break;
        }


        // Waiting for new configuration to take effect.
        if (_enablingGeospatial)
        {
            _configurePrepareTime -= Time.deltaTime;
            if (_configurePrepareTime < 0)
            {
                _enablingGeospatial = false;
            }
            else
            {
                return;
            }
        }

        // Check earth state.
        var earthState = EarthManager.EarthState;
        if (earthState == EarthState.ErrorEarthNotReady)
        {
            onDebugMessage.Invoke(_localizationInitializingMessage);
            return;
        }
        else if (earthState != EarthState.Enabled)
        {
            onLocationServiceError.Invoke();
            string errorMessage = "Geospatial sample encountered an EarthState error: " + earthState;
            onDebugMessage.Invoke(errorMessage);
            return;
        }


        // Check earth localization.
        bool isSessionReady = ARSession.state == ARSessionState.SessionTracking &&
            Input.location.status == LocationServiceStatus.Running;
        var earthTrackingState = EarthManager.EarthTrackingState;
        var pose = earthTrackingState == TrackingState.Tracking ?
            EarthManager.CameraGeospatialPose : new GeospatialPose();
        if (!isSessionReady || earthTrackingState != TrackingState.Tracking ||
            pose.OrientationYawAccuracy > _orientationYawAccuracyThreshold ||
            pose.HorizontalAccuracy > _horizontalAccuracyThreshold)
        {
            // Lost localization during the session.
            if (!isLocalizing)
            {
                isLocalizing = true;
                _localizationPassedTime = 0f;
            }

            if (_localizationPassedTime > _timeoutSeconds)
            {
                onDebugMessage.Invoke("Geospatial sample localization timed out.");
                CheckARSession.Instance.ReturnWithReason(_localizationFailureMessage);
            }
            else
            {
                _localizationPassedTime += Time.deltaTime;
                onDebugMessage.Invoke(_localizationInstructionMessage);
            }
        }
        else if (isLocalizing)
        {
            // Finished localization.
            isLocalizing = false;
            _localizationPassedTime = 0f;
            onDebugMessage.Invoke(_localizationSuccessMessage);
        }
        else
        {
            // Ready to start game
            onLocationServiceSuccess.Invoke();
            _localisationServiceOk = true;
            onDebugMessage.Invoke("Ready to start game");
        }
    }


    private IEnumerator StartLocationService()
    {
        waitingForLocationService = true;
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            onDebugMessage.Invoke("Requesting the fine location permission.");
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSeconds(3.0f);
        }
#endif

        if (!Input.location.isEnabledByUser)
        {
            onDebugMessage.Invoke("Location service is disabled by the user.");
            waitingForLocationService = false;
            yield break;
        }

        onDebugMessage.Invoke("Starting location service.");
        Input.location.Start();

        while (Input.location.status == LocationServiceStatus.Initializing)
        {
            yield return null;
        }

        waitingForLocationService = false;
        if (Input.location.status != LocationServiceStatus.Running)
        {
            onDebugMessage.Invoke("Location service ended with " + Input.location.status + " status.");
            Input.location.Stop();
        }
    }


    public void OnDisable()
    {
        StopCoroutine(_startLocationService);
        _startLocationService = null;
        onDebugMessage.Invoke("Stop location services.");
        Input.location.Stop();
    }
}
