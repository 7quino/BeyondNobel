using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotate : MonoBehaviour
{
    public float rotationSpeed = 10.0f; 
    void Update()
    {
        
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // Apply the rotation around the Y-axis
        transform.Rotate(Vector3.up, rotationAmount);
    }
}