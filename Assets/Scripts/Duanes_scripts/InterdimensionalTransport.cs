using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;

public class InterdimensionalTransport : MonoBehaviour
{
    public GameObject ARCamera;

    // Create an array of materials that you can assign in the Inspector.
    public Material[] EnvironmentMaterials;

    public GameObject Environment;

    void Start()
    {
        EnvironmentMaterials = Environment.GetComponent<Renderer>().sharedMaterials;
    }

    // Update is called once per frame
    void OnTriggerStay(Collider collider)
    {
        Vector3 camPositionInPortalSpace = transform.InverseTransformPoint(ARCamera.transform.position);

        if (camPositionInPortalSpace.y < 1.0f)
        {
            // Disable Stencil test
            for (int i = 0; i < EnvironmentMaterials.Length; ++i)
            {
                EnvironmentMaterials[i].SetInt("_StencilComp", (int)CompareFunction.Always);
            }
        }
        else
        {
            for (int i = 0; i < EnvironmentMaterials.Length; ++i)
            {
                EnvironmentMaterials[i].SetInt("_StencilComp", (int)CompareFunction.Equal);
            }
        }
    }
}

