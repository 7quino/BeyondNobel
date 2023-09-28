using UnityEngine;

public class MeasurementConversion : MonoBehaviour
{
    public float realWorldHeight = 63.2f; // Height in meters
    public float realWorldArea = 183.64f; // Area in square meters
    public float realWorldPerimeter = 183.62f; // Perimeter in meters
    public float realWorldLargeArea = 1906.54f; // Large area in square meters

    void Start()
    {
        // Calculate the scale factor for height
        float unityHeight = transform.localScale.y; // Assuming height is along the Y-axis
        float heightScale = realWorldHeight / unityHeight;

        // Calculate the scale factor for area
        float unityArea = transform.localScale.x * transform.localScale.z; // Assuming area is along the X and Z axes
        float areaScale = realWorldArea / unityArea;

        // Calculate the scale factor for perimeter
        float unityPerimeter = (transform.localScale.x + transform.localScale.z) * 2f; // Assuming a rectangular shape
        float perimeterScale = realWorldPerimeter / unityPerimeter;

        // Calculate the scale factor for the large area
        float unityLargeArea = transform.localScale.x * transform.localScale.z; // Assuming large area is along the X and Z axes
        float largeAreaScale = realWorldLargeArea / unityLargeArea;

        Debug.Log("Height Scale Factor: " + heightScale);
        Debug.Log("Area Scale Factor: " + areaScale);
        Debug.Log("Perimeter Scale Factor: " + perimeterScale);
        Debug.Log("Large Area Scale Factor: " + largeAreaScale);
    }
}