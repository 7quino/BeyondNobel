using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleMovement : MonoBehaviour
{
    public Transform startPoint; // The starting point in world space
    public Transform endPoint;   // The ending point in world space
    public float moveDuration = 2f; // Time taken to move from start to end points (in seconds)

    private float elapsedTime = 0f; // Time elapsed since the movement started

    void Update()
    {
        // Increment the elapsed time based on the time passed since the last frame
        elapsedTime += Time.deltaTime;

        // Calculate the interpolation factor (t) between 0 and 1 based on elapsed time and move duration
        float t = Mathf.Clamp01(elapsedTime / moveDuration);

        // Use Vector3.Lerp to move the object smoothly from start to end points
        transform.position = Vector3.Lerp(startPoint.position, endPoint.position, t);

        // If the object has reached the end point, reset the elapsed time
        if (t >= 1f)
        {
            elapsedTime = 0f;
        }
    }
}
