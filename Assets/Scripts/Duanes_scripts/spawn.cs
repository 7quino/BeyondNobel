using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class spawn : MonoBehaviour
{
    public GameObject spawn_prefab;

    GameObject spawned_object;

    bool object_spawned;

    ARRaycastManager raycastManager;

    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // Start is called before the first frame update
    void Start()
    {
        object_spawned = false;
        raycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && !object_spawned)
        {
            Touch touch = Input.GetTouch(0);
            if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;
                spawned_object = Instantiate(spawn_prefab, hitPose.position, hitPose.rotation);
                object_spawned = true;
            }
        }
        else if (Input.touchCount > 0 && object_spawned)
        {
            Touch touch = Input.GetTouch(0);
            if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;
                spawned_object.transform.position = hitPose.position;
            }
        }
    }
}
