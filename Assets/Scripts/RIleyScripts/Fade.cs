using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Fade : MonoBehaviour

 {
 
     [SerializeField]
   float duration;
 
   float t = 0f;
  Color color1 = Color.black, color2 = new Color(1f, 1f, 1f, 0f);
  MeshRenderer meshRenderer;
 
   void Start(){
    meshRenderer = GetComponent<MeshRenderer>();
   }
 
   void Update() {
    StartCoroutine(FadeLogo());
 
   }
 
  IEnumerator FadeLogo(){
     yield return new WaitForSeconds(18.7f); // wait time
 
    Color color = Color.Lerp(color1, color2, t);
    t += Time.deltaTime / duration;
     foreach (Material material in meshRenderer.materials) {
      material.color = color;
     }
   }
}

