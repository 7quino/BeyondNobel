using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class PortalManager : MonoBehaviour
{

    GameObject MainCamera;

    public Renderer[] Renderers;
    
    private List<Material> Materials = new List<Material>();

    
    // use this for initialization
    void Start()
    {
        foreach (var rend in Renderers) {
            Materials.Add(rend.material);
        }
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log($"{other.name} has entered the building!");
        MainCamera = Camera.main.gameObject;
    }
    void OnTriggerExit(Collider other) {
        Debug.Log($"{other.name} has left the building!");
    }

    // Update is called once per frame
    void OnTriggerStay (Collider collider)
    {
        Vector3 camPositionInPortalSpace = transform.InverseTransformPoint(MainCamera.transform.position);

        if(camPositionInPortalSpace.y < 1.0f)
        {
            //disable stencil test
            for(int i = 0; i < Materials.Count; ++i)
            {
                Materials[i].SetInt("_StencilComp", (int)CompareFunction.Always);
            }
        }
        else
        {
            //enable stencil test
            for (int i = 0; i < Materials.Count; ++i)
            {
                Materials[i].SetInt("_StencilComp", (int)CompareFunction.Equal);
            }

        }
    }
}
