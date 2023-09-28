using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceObjectGeospatialAnchor : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI infoText;
    [SerializeField] Button placePrefab;
    [SerializeField] Transform prefab;
    [SerializeField] ARAnchorManager anchorManager;
    [SerializeField] AREarthManager earthManager;
    [SerializeField] double longitude;
    [SerializeField] double latitude;
    [SerializeField] double altitude;
    [SerializeField] Quaternion eunRotation;

    ARGeospatialAnchor anchor;

    bool prefabPlaced = false;

    private void Start()
    {
        placePrefab.onClick.AddListener(() => OnPlacePrefab());

    }

    void OnPlacePrefab()
    {
        if (prefabPlaced) return;

        var earthTrackingState = earthManager.EarthTrackingState;
        if (earthTrackingState == TrackingState.Tracking)
        {
            anchor = ARAnchorManagerExtensions.AddAnchor(anchorManager, latitude, longitude, altitude, eunRotation);
            var anchoredAsset = Instantiate(prefab, anchor.transform);
            prefabPlaced = true;
            placePrefab.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (anchor != null)
        {
            GeospatialPose geopose = earthManager.Convert(anchor.pose);
            infoText.text = "Anchor position: " + anchor.transform.position +
                "\nAnchor rotation: " + anchor.transform.rotation +
                "\n" + longitude + " vs " + geopose.Longitude +
                "\n" + latitude + " vs " + geopose.Latitude +
                "\n" + altitude + " vs " + geopose.Altitude;
        }
    }
}
