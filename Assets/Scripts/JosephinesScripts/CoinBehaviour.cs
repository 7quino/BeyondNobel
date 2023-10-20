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
    bool isGrowing;
    Transform square;

    void Start()
    {
        square = GameObject.FindGameObjectWithTag("Square").transform;
        audioSource = GetComponent<AudioSource>();
        coinClip = coinClips[Random.Range(0, coinClips.Count - 1)];
        audioSource.clip = coinClip;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Square" || collision.gameObject.tag == "Coin")
        {
            audioSource.PlayOneShot(coinClip);
        }

        if (collision.gameObject.tag == "Square")
        {
            isGrowing = true;
            this.GetComponent<Rigidbody>().isKinematic = true;

            _ = topPivot.position.y < bottomPivot.position.y ? StartCoroutine(GrowCoin(coinBottom)) : StartCoroutine(GrowCoin(coinTop));
        }
    }

    IEnumerator GrowCoin(Transform coinTransform)
    {
        Vector3 coinEndSize = new Vector3( 1, Random.Range(20.0f, 1500.0f), 1);
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

    private void Update()
    {
        if (isGrowing) return;

        if (transform.position.y < (square.transform.position.y - 1))
        {
            Destroy(gameObject);
            Debug.Log("destroyed");
        }
    }
}
