using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabEconomics : MonoBehaviour
{
    [SerializeField] GameObject coinPrefab;
    [SerializeField] Transform coinHolder;
    [SerializeField] int numberCoins;
    [SerializeField] float timeBetweenSpawn;
    [SerializeField] float xzForce;
    [SerializeField] float yForce;

    List<GameObject> coins = new List<GameObject>();
    IEnumerator spawnCoinsRoutine;

    public void OnButtonClickActivate()
    {
        spawnCoinsRoutine = SpawnCoins();
        StartCoroutine(spawnCoinsRoutine);
    }

    public void OnButtonClickDeactivate()
    {
        if (spawnCoinsRoutine != null) StopCoroutine(spawnCoinsRoutine);

        if (coins.Count > 0)
        {
            foreach (var coin in coins)
            {
                Destroy(coin);
            }

            coins.Clear();
        }
    }

    IEnumerator SpawnCoins()
    {
        for (int i = 0; i < numberCoins; i++)
        {
            GameObject coin = Instantiate(coinPrefab, coinHolder);
            coin.transform.localPosition = Vector3.zero;

            float forceX = Random.Range(-xzForce, xzForce);
            float forceZ = Random.Range(-xzForce, xzForce);
            float forceY = Random.Range(1, yForce);

            coin.GetComponent<Rigidbody>().AddForce(new Vector3( forceX, forceY, forceZ), ForceMode.Impulse);
            coins.Add(coin);

            yield return new WaitForSeconds(timeBetweenSpawn);
        }
    }
}
