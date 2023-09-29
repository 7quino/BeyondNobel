using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    public float dissolveDuration = 6;
    public float dissolveStrength;

    private Material dissolveMaterial; // Cache the material reference.

    private void Start()
    {
        // Assuming the material is on the same GameObject as this script.
        dissolveMaterial = GetComponent<Renderer>().material;
        // Initialize dissolveStrength to 0.
        dissolveStrength = 0;
    }

    public void OnButtonClickActivate()
    {
        StartCoroutine(dissolver());
    }

    public IEnumerator dissolver()
    {
        float elapsedTime = 0;

        while (elapsedTime < dissolveDuration)
        {
            elapsedTime += Time.deltaTime;

            dissolveStrength = Mathf.Lerp(0, 1, elapsedTime / dissolveDuration);
            dissolveMaterial.SetFloat("_DissolveStrength", dissolveStrength);

            yield return null;
        }
    }
}

