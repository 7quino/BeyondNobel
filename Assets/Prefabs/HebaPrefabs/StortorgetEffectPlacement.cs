using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class StortorgetEffectPlacement : MonoBehaviour
{
    public Button prizeButtonChemistry; // Reference to the PrizeButtonChemistry button
    public GameObject chemistryEffectPrefab; // Reference to the chemistryeffect prefab

    private ARRaycastManager arRaycastManager;
    private Vector3 targetLocation; // The target location for the VFX
    private bool isEffectActive = false; // Boolean to control the effect's activation state

    private void Start()
    {
        // Initialize the ARRaycastManager
        arRaycastManager = GetComponent<ARRaycastManager>();

        // Assign an event handler to the button click
        prizeButtonChemistry.onClick.AddListener(ToggleEffect);

        // Convert geographic coordinates to Unity world space
        Vector2 stortorgetCoordinates = new Vector2(59.3296f, 18.0686f); // Latitude and longitude of Stortorget
        targetLocation = ConvertGeographicToARWorldSpace(stortorgetCoordinates, 63.2f); // Height is 63.2 meters
    }

    private void ToggleEffect()
    {
        // Toggle the effect's activation state
        isEffectActive = !isEffectActive;

        if (isEffectActive)
        {
            // Perform a raycast to place the VFX at the target location
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (arRaycastManager.Raycast(targetLocation, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                Instantiate(chemistryEffectPrefab, hitPose.position, Quaternion.identity);
            }
        }
        else
        {
            // Deactivate the effect if it's currently active
            GameObject effectInstance = GameObject.FindGameObjectWithTag("ChemistryEffect"); // Assuming "ChemistryEffect" is the tag of your chemistryeffect prefab
            if (effectInstance != null)
            {
                effectInstance.SetActive(false);
            }
        }
    }

    Vector3 ConvertGeographicToARWorldSpace(Vector2 geoCoordinates, float height)
    {
        // Implement the conversion logic to map geographic coordinates to AR world space.
        // This will depend on your chosen scale and coordinate system.
        // You may need to apply a scaling factor and offset to align the coordinates correctly.

        // For a simple example, you can assume a flat Earth and a fixed scale.
        // You may need to adjust this for a more accurate representation.
        float latitudeToMeters = 111131.75f;
        float longitudeToMeters = 111131.75f;
        Vector3 position = new Vector3(
            (geoCoordinates.y - 18.0686f) * longitudeToMeters,
            height,
            (geoCoordinates.x - 59.3296f) * latitudeToMeters
        );

        return position;
    }
}

