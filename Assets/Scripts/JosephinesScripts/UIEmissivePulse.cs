using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEmissivePulse : MonoBehaviour
{
    [SerializeField] Material pulseMaterial;
    [SerializeField] Color _emissionColorValue;
    [SerializeField] float lowIntensity;
    [SerializeField] float highIntensity;
    [SerializeField] float pulseTime;
    bool increaseIntensity = true;

    private void OnEnable()
    {
        StartCoroutine(pulseEmission(increaseIntensity));
    }

    IEnumerator pulseEmission(bool increase)
    {
        float timeElapsed = Time.deltaTime;
        float stopTime = pulseTime + Time.deltaTime;

        while (timeElapsed < stopTime)
        {
            float _intensity = increaseIntensity == true ? Mathf.Lerp(lowIntensity, highIntensity, timeElapsed / stopTime) : Mathf.Lerp(highIntensity, lowIntensity, timeElapsed / stopTime);
            pulseMaterial.SetColor("_Color", _emissionColorValue * _intensity);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        increaseIntensity = !increaseIntensity;
        StartCoroutine(pulseEmission(increaseIntensity));
    }
}
