using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{
    [SerializeField] List<AudioClip> coinClips;
    [SerializeField] Transform coinTop;
    [SerializeField] Transform coinBottom;
    [SerializeField] Transform topPivot;
    [SerializeField] Transform bottomPivot;

    AudioSource audioSource;
    AudioClip coinClip;

    private void OnCollisionEnter(Collision collision)
    {
        audioSource.PlayOneShot(coinClip);

        if (collision.gameObject.tag == "Square")
        {
            
            this.GetComponent<Rigidbody>().isKinematic = true;

            _ = topPivot.position.y < bottomPivot.position.y ? StartCoroutine(GrowCoin(coinBottom)) : StartCoroutine(GrowCoin(coinTop));
        }
    }


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        coinClip = coinClips[Random.Range(0, coinClips.Count - 1)];
        audioSource.clip = coinClip;
    }

    IEnumerator GrowCoin(Transform coinTransform)
    {
        Vector3 coinEndSize = new Vector3( 1, Random.Range(50.0f, 5000.0f), 1);
        Vector3 coinStartSize = Vector3.one;
        float elapsedTime = Time.deltaTime;
        float endTime = Time.deltaTime + Random.Range(30, 300);


        while (elapsedTime < endTime)
        {
            coinTransform.localScale = Vector3.Lerp(coinStartSize, coinEndSize, elapsedTime/endTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
