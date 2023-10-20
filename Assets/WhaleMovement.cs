using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleMovement : MonoBehaviour
{
    public float speed;

    void Start()
    {

    }

    void Update() 
    {
        transform.Translate(0, 0, 1 * speed * Time.deltaTime);
    }



}
