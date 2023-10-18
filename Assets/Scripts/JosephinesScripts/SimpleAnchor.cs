using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SimpleAnchor : MonoBehaviour
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
    protected ARAnchorManager AnchorManager;
    protected ARGeospatialAnchor anchorGeo = null;
    protected GameObject anchoredAsset = null;
    protected bool locationServiceFailure = false;
    protected bool buttonIsActive = false;

    public static SimpleAnchor instance;
    public UnityEvent<string> OnSendMessage = new UnityEvent<string>();

    private void Awake()
    {
        instance = this;
    }


    private void Update()
    {
        if (anchoredAsset != null) return;

        var earthTrackingState = earthManager.EarthTrackingState;
        if (earthTrackingState == TrackingState.Tracking)
        {
            var anchor =
                AnchorManager.AddAnchor(
                    latitude,
                    longitude,
                    altitude,
                    quaternion);
            var anchoredAsset = Instantiate(anchorPrefab, anchor.transform);
        }

    }


}
