using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PortalManager : MonoBehaviour
{
    GameObject MainCamera;
    public Renderer[] Renderers;
    private List<Material> Materials = new List<Material>();
    public AudioClip audioClip;
    private AudioSource audioSource;

    // Use this for initialization
    void Start()
    {
        foreach (var rend in Renderers)
        {
            Materials.Add(rend.material);
        }

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.name} has entered the building!");
        MainCamera = Camera.main.gameObject;

        // Play audio track when collision occurs
        if (audioClip != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log($"{other.name} has left the building!");

        // Stop playing audio track on exit
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // Update is called once per frame
    void OnTriggerStay(Collider collider)
    {
        Vector3 camPositionInPortalSpace = transform.InverseTransformPoint(MainCamera.transform.position);

        if (camPositionInPortalSpace.y <= 0.0f)
        {
            for (int i = 0; i < Materials.Count; ++i)
            {
                Materials[i].SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            }
        }
        else if (camPositionInPortalSpace.y < 0.5f)
        {
            // Disable stencil test
            for (int i = 0; i < Materials.Count; ++i)
            {
                Materials[i].SetInt("_StencilComp", (int)CompareFunction.Always);
            }
        }
        else
        {
            // Enable stencil test
            for (int i = 0; i < Materials.Count; ++i)
            {
                Materials[i].SetInt("_StencilComp", (int)CompareFunction.Equal);
            }
        }
    }
}
