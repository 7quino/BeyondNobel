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

    protected ARRaycastManager arRaycastManager;
    protected AREarthManager earthManager;
    protected ARAnchorManager anchorManager;
    protected ARGeospatialAnchor anchorGeo = null;
    protected GameObject anchorPoint = null;
    protected GameObject anchoredAsset = null;
    protected bool locationServiceSuccess = false;
    protected bool locationServiceFailure = false;
    protected bool buttonIsActive = false;

    protected void Start()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        earthManager = FindObjectOfType<AREarthManager>();
        anchorManager = FindObjectOfType<ARAnchorManager>();

        //CheckLocationService.Instance.onLocationServiceSuccess.AddListener(PlaceAnchor);
        CheckLocationService.Instance.onLocationServiceSuccess.AddListener(() => locationServiceSuccess = true);
        CheckLocationService.Instance.onLocationServiceError.AddListener(() => locationServiceFailure = true);
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
            //anchorPoint.transform.position = anchorGeo.transform.position;

            //For testing
            //ShowButton();
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


        if (anchorGeo == null && anchoredAsset == null)
        {

            string message = LanguageManager.instance._localeID == 0 ? "Wait for location!" : "Vänta på platsen!";
            UiManager.instance.locationServiceMessage.text = message;
        }

        if (locationServiceFailure && anchoredAsset == null)
        {
            string message = LanguageManager.instance._localeID == 0 ? "tap to place experience!" : "Tryck för att placera upplevelsen!";
            UiManager.instance.ShowMessage(message);
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
