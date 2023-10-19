using System.Collections;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class AnchorPlacePrefab : MonoBehaviour
{
    [Header("AR anchor")]
    [SerializeField] double latitude;
    [SerializeField] double longitude;
    [SerializeField] double altitude = 38;
    [SerializeField] Quaternion quaternion;
    [SerializeField] protected GameObject anchorPrefab;

    [SerializeField] protected string userDirection = string.Empty;
    [SerializeField] protected bool showDirectly = false;

    protected ARRaycastManager arRaycastManager;
    protected AREarthManager earthManager;
    protected ARAnchorManager anchorManager;
    protected ARGeospatialAnchor anchorGeo = null;
    protected GameObject anchorPoint = null;
    protected GameObject anchoredAsset = null;
    protected bool privacyPromptOkay = false;
    protected bool locationServiceSuccess = false;
    protected bool locationServiceFailure = false;
    protected bool buttonIsActive = false;

    protected void Start()
    {
        CheckLocationService.Instance.onLocationServiceSuccess.AddListener(() => locationServiceSuccess = true);
        CheckLocationService.Instance.onLocationServiceError.AddListener(() => locationServiceFailure = true);
        UiManager.instance.onPrivacyPromptIsOk.AddListener((enable) => privacyPromptOkay = true);

        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        earthManager = FindObjectOfType<AREarthManager>();
        anchorManager = FindObjectOfType<ARAnchorManager>();
    }

    void Update()
    {
        PlaceAnchor();

        if (!locationServiceFailure) return;
        if (anchoredAsset != null) return;
        if (!buttonIsActive) return;

        Touch touch = Input.GetTouch(0);
        Vector2 touchPos = touch.position;
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        if (arRaycastManager.Raycast(touchPos, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            anchoredAsset = Instantiate(anchorPrefab, hitPose.position, Quaternion.identity);
        }
    }

    public void PlaceAnchor()
    {
        if (!privacyPromptOkay) return;
        if (!locationServiceSuccess) return;
        if (anchorGeo != null) return;


        var earthTrackingState = earthManager.EarthTrackingState;
        if (earthTrackingState == TrackingState.Tracking)
        {
            //For testing at camera altitude
            var cameraGeospatialPose = earthManager.CameraGeospatialPose;

            anchorGeo = ARAnchorManagerExtensions.AddAnchor(
                    anchorManager,
                    latitude,
                    longitude,
                    altitude,
                    //cameraGeospatialPose.Altitude,
                    quaternion);

            anchorPoint = Instantiate(new GameObject(), anchorGeo.transform);

            if (showDirectly)
            {
                ShowButton();
            }
        }
    }

    public virtual void ShowButton()
    {

#if UNITY_EDITOR
        anchoredAsset = Instantiate(anchorPrefab, new Vector3(0,0,4), Quaternion.identity);
#endif

        buttonIsActive = true;

        if (anchorGeo && anchoredAsset == null)
        {
            anchoredAsset = Instantiate(anchorPrefab, anchorPoint.transform);
        }

        if (userDirection != string.Empty)
        {
            UiManager.instance.ShowMessage(userDirection);
        }


        if (anchorGeo == null && anchoredAsset == null)
        {
            UiManager.instance.locationServiceMessage.text = LanguageManager.instance._localeID == 0 ? "Wait for location!" : "V�nta p� platsen!";
        }

        if (locationServiceFailure && anchoredAsset == null)
        {
            UiManager.instance.ShowMessage(LanguageManager.instance._localeID == 0 ? "tap to place experience!" : "Tryck f�r att placera upplevelsen!");
        }
        else if (locationServiceFailure)
        {
            anchoredAsset.SetActive(true);
        }
    }

    public virtual void HideButton()
    {
#if UNITY_EDITOR
        Destroy(anchoredAsset);
#endif

        buttonIsActive = false;

        if (anchoredAsset == null) return;

        if (!locationServiceFailure)
        {
            Destroy(anchoredAsset);
        }
        else
        {
            anchoredAsset.SetActive(false);
        }
    }

    protected IEnumerator PlaceWithPlaneTracking()
    {
        yield return null;
    }
}
