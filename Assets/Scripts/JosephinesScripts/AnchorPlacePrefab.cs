using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AnchorPlacePrefab : MonoBehaviour
{
    [Header("AR anchor")]
    [SerializeField] AREarthManager earthManager;
    [SerializeField] ARAnchorManager anchorManager;
    [SerializeField] double latitude;
    [SerializeField] double longitude;
    [SerializeField] double altitude;
    [SerializeField] Quaternion quaternion;
    public GameObject anchorPrefab;
    bool coinHolderAnchored = false;

    public void PlaceAnchor()
    {
        if (coinHolderAnchored) return;



        var earthTrackingState = earthManager.EarthTrackingState;
        if (earthTrackingState == TrackingState.Tracking)
        {
            var cameraGeospatialPose = earthManager.CameraGeospatialPose;
            //debugtext.text = "\n" + cameraGeospatialPose.Altitude;

            var anchorGeo = ARAnchorManagerExtensions.AddAnchor(
                    anchorManager,
                    latitude,
                    longitude,
                    cameraGeospatialPose.Altitude,
                    quaternion);

            var anchoredAsset = Instantiate(anchorPrefab, anchorGeo.transform);
            anchoredAsset.transform.position = anchorGeo.transform.position;

            coinHolderAnchored = true;
        }
    }
}
