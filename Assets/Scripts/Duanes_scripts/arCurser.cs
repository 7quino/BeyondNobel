using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class arCurser : MonoBehaviour
{
    public GameObject curserChildObject;
    public GameObject objectToPlace;
    public ARRaycastManager raycastManager;

    private bool useCurser = true; // Define the useCurser variable at the class level
    private bool hasObjectSpawned = false;

    // Start is called before the first frame update
    void Start()
    {
        curserChildObject.SetActive(useCurser);
    }

    // Update is called once per frame
    void Update()
    {
        if (hasObjectSpawned) return;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (useCurser)
            {
                Instantiate(objectToPlace, transform.position, transform.rotation);
                hasObjectSpawned = true;
                curserChildObject.SetActive(false);
            }
            else
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                raycastManager.Raycast(Input.GetTouch(0).position, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);
                if (hits.Count > 0)
                {
                    Instantiate(objectToPlace, hits[0].pose.position, hits[0].pose.rotation);
                    hasObjectSpawned = true;
                    curserChildObject.SetActive(false);
                }
            }
        }
    }
}

