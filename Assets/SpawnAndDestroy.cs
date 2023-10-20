using System.Collections;
using UnityEngine;

public class SpawnAndDestroy : MonoBehaviour
{
    public GameObject[] objectsToSpawn;
    private GameObject spawnedObject;
    private bool cameraInsideCollider = false;

    void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the AR Camera (MainCamera)
        if (other.CompareTag("MainCamera"))
        {
            // If camera is inside the collider, don't spawn a new object
            if (cameraInsideCollider)
            {
                return;
            }

            // Spawn a random object from the array at the current position and rotation of the SpawnAndDestroy object
            GameObject objectToSpawn = objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];
            spawnedObject = Instantiate(objectToSpawn, transform.position, transform.rotation);
            cameraInsideCollider = true;

            // You can do additional setup for the spawned object if needed
            // For example, you might want to attach the spawned object to a parent:
            // spawnedObject.transform.parent = transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the colliding object is the AR Camera (MainCamera)
        if (other.CompareTag("MainCamera"))
        {
            // If camera is inside the collider, mark it as exited, but don't destroy the object yet
            if (cameraInsideCollider)
            {
                cameraInsideCollider = false;
            }
            // If the camera has exited the collider again, destroy the spawned object
            else if (spawnedObject != null)
            {
                Destroy(spawnedObject);
            }
        }
    }
}

