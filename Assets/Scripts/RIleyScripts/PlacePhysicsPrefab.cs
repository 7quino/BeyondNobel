using System.Collections;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlacePhysicsPrefab : MonoBehaviour
{
    [Header("AR anchor")]
    [SerializeField] AREarthManager earthManager;
    [SerializeField] ARAnchorManager anchorManager;
    [SerializeField] double latitude;
    [SerializeField] double longitude;
    [SerializeField] double altitude = 38;
    [SerializeField] Quaternion quaternion;
    [SerializeField] GameObject anchorPrefab;
    bool prefabAnchored = false;

    [SerializeField] Transform sun;





    public void PlaceAnchor()
    {
        if (prefabAnchored) return;

        var earthTrackingState = earthManager.EarthTrackingState;
        if (earthTrackingState == TrackingState.Tracking)
        {
            //var cameraGeospatialPose = earthManager.CameraGeospatialPose;
            //debugtext.text = "\n" + cameraGeospatialPose.Altitude;

            var anchorGeo = ARAnchorManagerExtensions.AddAnchor(
                    anchorManager,
                    latitude,
                    longitude,
                    altitude,
                    quaternion);

            var anchoredAsset = Instantiate(anchorPrefab, anchorGeo.transform);
            anchoredAsset.transform.position = anchorGeo.transform.position;
            //sun = GameObject.Find("Sun").GetComponent<Transform>();

            prefabAnchored = true;
        }
    }





}
