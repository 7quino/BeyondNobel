using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

public class InterdimensionalTransport : MonoBehaviour
{
    public Material[] materials;


    // Start is called before the first frame update
    void Start()
    {
        foreach (var mat in materials)
        {
            mat.SetInt("_StencilTest", (int)CompareFunction.Equal);
        }
    }

    void OntriggerStay(Collider other)
    {
        if (other.name != "MainCamera")
            return;


        // Outside of other world
        if (transform.position.z > other.transform.position.z)
        {
            UnityEngine.Debug.Log("Outside of other world");
            foreach (var mat in materials)
            {
                mat.SetInt("_StencilTest", (int)CompareFunction.Equal);
            }
            //Inside other dimension
        }
        else
        {
            UnityEngine.Debug.Log("Inside other world");
            foreach (var mat in materials)
            {
                mat.SetInt("_StencilTest", (int)CompareFunction.NotEqual);
            }   
          
        }

    }
        
    void OnDestroy() 
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        foreach (var mat in materials)
        {
            mat.SetInt("_StencilTest", (int)CompareFunction.NotEqual);
        }
    }
}
