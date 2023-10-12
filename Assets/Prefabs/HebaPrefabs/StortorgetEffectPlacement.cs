using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions;

public class StortorgetEffectPlacement : MonoBehaviour
{
    [Header("AR anchor")]
    [SerializeField] AREarthManager earthManager;
    [SerializeField] ARAnchorManager anchorManager;
    [SerializeField] double latitude;
    [SerializeField] double longitude;
    [SerializeField] double altitude = 38;
    [SerializeField] Quaternion quaternion;
    [SerializeField] GameObject chemistryEffectPrefab; // Reference to the chemistryeffect prefab


    GameObject chemistryPrefabInstance = null;

    [SerializeField] ARRaycastManager arRaycastManager;
    private Vector3 targetLocation; // The target location for the VFX


    private void Start()
    {
        
    }


    public void PlaceAnchor()
    {
        if (chemistryPrefabInstance != null) return;

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

            chemistryPrefabInstance = Instantiate(chemistryEffectPrefab, anchorGeo.transform);
            chemistryPrefabInstance.transform.position = anchorGeo.transform.position;
            chemistryPrefabInstance.SetActive(false);
        }
    }


    public void ActivateEffect()
    {
        if (chemistryPrefabInstance != null) 
        {
            chemistryPrefabInstance.SetActive(true);
            return;
        }
        else
        {

            //for placing with plane tracking
            // Perform a raycast to place the VFX at the target location
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (arRaycastManager.Raycast(targetLocation, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;

                // Instantiate the effect
                Instantiate(chemistryEffectPrefab, hitPose.position, Quaternion.identity);
            }
        }
    }


    public void DeactivateEffect()
    {
        if (chemistryPrefabInstance == null) return;

        chemistryPrefabInstance.SetActive(false);
    }
}

