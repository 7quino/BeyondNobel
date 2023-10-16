using Google.XR.ARCoreExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PrefabEconomics : MonoBehaviour
{
    [Header("Prefab behaviour")]
    [SerializeField] GameObject coinPrefab;
    [SerializeField] int numberCoins;
    [SerializeField] float timeBetweenSpawn;
    [SerializeField] float xzForce;
    [SerializeField] float yForce;
    [SerializeField] Transform coinHolder;

    List<GameObject> coins = new List<GameObject>();
    IEnumerator spawnCoinsRoutine;


    private void Start()
    {
        spawnCoinsRoutine = SpawnCoins();
        StartCoroutine(spawnCoinsRoutine);
    }

    IEnumerator SpawnCoins()
    {
        for (int i = 0; i < numberCoins; i++)
        {
            GameObject coin = Instantiate(coinPrefab, coinHolder);
            coin.transform.localPosition = Vector3.zero;

            float forceX = UnityEngine.Random.Range(-xzForce, xzForce);
            float forceZ = UnityEngine.Random.Range(-xzForce, xzForce);
            float forceY = UnityEngine.Random.Range(1, yForce);

            coin.GetComponent<Rigidbody>().AddForce(new Vector3( forceX, forceY, forceZ), ForceMode.Impulse);
            coins.Add(coin);

            yield return new WaitForSeconds(timeBetweenSpawn);
        }
    }
}
