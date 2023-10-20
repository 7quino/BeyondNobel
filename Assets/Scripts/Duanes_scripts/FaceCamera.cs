using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    void Update()
    {
        // Get the position of the main camera
        Vector3 cameraPosition = Camera.main.transform.position;

        // Make the object look at the camera, ignoring the camera's rotation on the y-axis
        transform.LookAt(new Vector3(cameraPosition.x, transform.position.y, cameraPosition.z));
    }
}
