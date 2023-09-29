using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    public float dissolveDuration = 6;
    public float dissolveStrength;

    private Material dissolveMaterial; 

    private void Start()
    {
        
        dissolveMaterial = GetComponent<Renderer>().material;
        dissolveStrength = 0;


        PrizeCategoryButtons.instance.prizeCategoryButtons[0].onButtonClickEnable.AddListener(OnButtonClickActivate);
        //PrizeCategoryButtons.instance.prizeCategoryButtons[0].onButtonClickDisable.AddListener();



        //For debug
        //StartCoroutine(dissolver());
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

