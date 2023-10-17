using System.Collections;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using TMPro;
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
    //[SerializeField] TextMeshProUGUI debugtext;

    protected ARRaycastManager arRaycastManager;
    protected AREarthManager earthManager;
    protected ARAnchorManager anchorManager;
    protected ARGeospatialAnchor anchorGeo = null;
    protected GameObject anchoredAsset = null;
    protected bool locationServiceFailure = false;
    protected bool buttonIsActive = false;

    protected void Start()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        earthManager = FindObjectOfType<AREarthManager>();
        anchorManager = FindObjectOfType<ARAnchorManager>();

        CheckLocationService.Instance.onLocationServiceSuccess.AddListener(PlaceAnchor);
        CheckLocationService.Instance.onLocationServiceError.AddListener(() => locationServiceFailure = true);
    }


    public void PlaceAnchor()
    {
        if (anchorGeo != null) return;

        var earthTrackingState = earthManager.EarthTrackingState;
        if (earthTrackingState == TrackingState.Tracking)
        {
            //For getting altitude camera
            var cameraGeospatialPose = earthManager.CameraGeospatialPose;
            //debugtext.text = "\n" + cameraGeospatialPose.Altitude;

            anchorGeo = ARAnchorManagerExtensions.AddAnchor(
                    anchorManager,
                    latitude,
                    longitude,
                    altitude,
                    //cameraGeospatialPose.Altitude,
                    quaternion);

            //For testing
            //ShowButton();
        }
    }

    public virtual void ShowButton()
    {

        buttonIsActive = true;

        if (anchorGeo && anchoredAsset == null)
        {
            anchoredAsset = Instantiate(anchorPrefab, anchorGeo.transform);
            anchoredAsset.transform.position = anchorGeo.transform.position;
        }


        if (anchorGeo == null && anchoredAsset == null)
        {
            UiManager.instance.locationServiceMessage.text = "Still searching\nfor location!";
        }

        if (locationServiceFailure && anchoredAsset == null)
        {
            UiManager.instance.ShowMessage("Tap to place experience!");

            //Exchange update function to coroutine
            //StartCoroutine(PlaceWithPlaneTracking());
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


    void Update()
    {
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
}
