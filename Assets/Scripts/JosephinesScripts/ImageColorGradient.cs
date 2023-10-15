using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageColorGradient : MonoBehaviour
{
    [SerializeField] Gradient colorPalett;
    [SerializeField] float speed;
    Image image;


    void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        float colorPosition = Mathf.PingPong(Time.time * speed, 1);
        image.color = colorPalett.Evaluate(colorPosition);
    }
}
