using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public float alpha = 20.0f;
    private Renderer r;// = GetComponent<Renderer>();
    void Start()
    {
       r  = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        var material = r.material;
        var color = material.color;
        material.color = new Color(color.r, color.g, color.b, alpha);
    }
}