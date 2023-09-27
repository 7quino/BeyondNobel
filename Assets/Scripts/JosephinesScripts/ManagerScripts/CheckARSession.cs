using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Events;
using UnityEngine.Android;

public class CheckARSession : MonoBehaviour
{
    public static CheckARSession Instance;
    [HideInInspector] public UnityEvent<string> OnArSessionStopped = new UnityEvent<string>();

    public ARSessionOrigin SessionOrigin;
    public ARSession Session;
    public ARCoreExtensions ARCoreExtensions;

    [HideInInspector] public bool isReturning = false;

    private const float _errorDisplaySeconds = 3;
    private IEnumerator _asyncCheck = null;

    private void Awake()
    {
        Instance = this;
    }

    public void OnEnable()
    {
        isReturning = false;
    }

    private void Update()
    {
        LifecycleUpdate();
    }

    private void LifecycleUpdate()
    {
        // Pressing 'back' button quits the app.
        if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
        if (isReturning) return;

        // Only allow the screen to sleep when not tracking.
        var sleepTimeout = SleepTimeout.NeverSleep;
        if (ARSession.state != ARSessionState.SessionTracking) sleepTimeout = SleepTimeout.SystemSetting;
        Screen.sleepTimeout = sleepTimeout;

        // Quit the app if ARSession is in an error status.
        string returningReason = string.Empty;
        if (ARSession.state != ARSessionState.CheckingAvailability &&
            ARSession.state != ARSessionState.Ready &&
            ARSession.state != ARSessionState.SessionInitializing &&
            ARSession.state != ARSessionState.SessionTracking)
        {
            returningReason = string.Format(
                "Geospatial sample encountered an ARSession error state {0}.\n" +
                "Please restart the app.",
                ARSession.state);
        }
        else if (Input.location.status == LocationServiceStatus.Failed)
        {
            returningReason =
                "Geospatial sample failed to start location service.\n" +
                "Please restart the app and grant the fine location permission.";
        }
        else if (SessionOrigin == null || Session == null || ARCoreExtensions == null)
        {
            returningReason = string.Format(
                "Geospatial sample failed due to missing AR Components.");
        }

        ReturnWithReason(returningReason);
    }

    public void ReturnWithReason(string reason)
    {
        if (string.IsNullOrEmpty(reason)) return;

        OnArSessionStopped.Invoke(reason);
        isReturning = true;
        Invoke(nameof(QuitApplication), _errorDisplaySeconds);
    }

    private void QuitApplication()
    {
        Application.Quit();
    }

    public void CheckAvilability()
    {
        if (_asyncCheck != null) return;

        _asyncCheck = AvailabilityCheck();
        StartCoroutine(_asyncCheck);
    }

    private IEnumerator AvailabilityCheck()
    {
        if (ARSession.state == ARSessionState.None)
        {
            yield return ARSession.CheckAvailability();
        }

        // Waiting for ARSessionState.CheckingAvailability.
        yield return null;

        if (ARSession.state == ARSessionState.NeedsInstall)
        {
            yield return ARSession.Install();
        }

        // Waiting for ARSessionState.Installing.
        yield return null;
#if UNITY_ANDROID

        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Debug.Log("Requesting camera permission.");
            Permission.RequestUserPermission(Permission.Camera);
            yield return new WaitForSeconds(3.0f);
        }

        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            // User has denied the request.
            Debug.LogWarning(
                "Failed to get the camera permission. VPS availability check isn't available.");
            yield break;
        }
#endif

        while (CheckLocationService.Instance.waitingForLocationService)
        {
            yield return null;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogWarning(
                "Location services aren't running. VPS availability check is not available.");
            yield break;
        }

        // Update event is executed before coroutines so it checks the latest error states.
        if (CheckARSession.Instance.isReturning) yield break;

        var location = Input.location.lastData;
        var vpsAvailabilityPromise = AREarthManager.CheckVpsAvailabilityAsync(location.latitude, location.longitude);
        yield return vpsAvailabilityPromise;

        Debug.LogFormat("VPS Availability at ({0}, {1}): {2}", location.latitude, location.longitude, vpsAvailabilityPromise.Result);
        UiManager.instance.vpsCheckCanvas.SetActive(vpsAvailabilityPromise.Result != VpsAvailability.Available);
    }

    public void OnDisable()
    {
        StopCoroutine(_asyncCheck);
        _asyncCheck = null;
    }
}
