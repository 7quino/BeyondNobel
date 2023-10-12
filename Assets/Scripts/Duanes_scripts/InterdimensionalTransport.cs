using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;

public class InterdimensionalTransport : MonoBehaviour
{
    public GameObject ARCamera;

    // Create an array of materials that you can assign in the Inspector.
    public Material[] EnvironmentMaterials;

    void Start()
    {
        foreach (var mat in EnvironmentMaterials)
        {
            mat.SetInt("_StencilTest", (int)CompareFunction.Equal);
        }
    }

    // Update is called once per frame
    void OnTriggerStay(Collider other)
    {


        if (other.CompareTag("MainCamera"))
            return;


        // outside other world
        if (transform.position.z > other.transform.position.z)
        {
           
            foreach (var mat in EnvironmentMaterials)
            {
                mat.SetInt("_StencilTest", (int)CompareFunction.Equal);
            }
            //inside other dimension
        }
        else
        {
            
            foreach (var mat in EnvironmentMaterials)
            {
                mat.SetInt("_StencilTest", (int)CompareFunction.NotEqual);
            }
        }
    }

    void Ondestroy()
    {
        foreach (var mat in EnvironmentMaterials)
        {
            mat.SetInt("_StencilTest", (int)CompareFunction.NotEqual);
        }
    }

    void Update()
    {

    }



}

