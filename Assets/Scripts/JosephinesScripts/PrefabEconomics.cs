using Google.XR.ARCoreExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class PrefabEconomics : MonoBehaviour
{
    [Header("AR anchor")]
    [SerializeField] AREarthManager earthManager;
    [SerializeField] ARAnchorManager anchorManager;
    [SerializeField] double latitude;
    [SerializeField] double longitude;
    [SerializeField] double altitude;
    [SerializeField] Quaternion quaternion;
    //[SerializeField] GameObject economicsPrefab;
    bool coinHolderAnchored = false;


    [Header("Prefab behaviour")]
    [SerializeField] GameObject coinPrefab;
    [SerializeField] Transform coinHolder;
    [SerializeField] int numberCoins;
    [SerializeField] float timeBetweenSpawn;
    [SerializeField] float xzForce;
    [SerializeField] float yForce;

    public TextMeshProUGUI debugtext;


    List<GameObject> coins = new List<GameObject>();
    IEnumerator spawnCoinsRoutine;

    public void OnButtonClickActivate()
    {
        try {
            if (!coinHolderAnchored)
            {
                var earthTrackingState = earthManager.EarthTrackingState;
                if (earthTrackingState == TrackingState.Tracking)
                {
                    var anchor =
                        ARAnchorManagerExtensions.AddAnchor(
                            anchorManager,
                            latitude,
                            longitude,
                            altitude,
                            quaternion);
                    coinHolder.parent = anchor.transform;
                    coinHolder.localPosition = Vector3.zero;
                    coinHolderAnchored = true;

                    debugtext.text = "\n Geoanchor fixed";
                }
                else
                {
                    //Place with plane tracking
                    debugtext.text = "\n Place with plane tracking";
                }
            }

            spawnCoinsRoutine = SpawnCoins();
            StartCoroutine(spawnCoinsRoutine);
            debugtext.text = "\n Economics anchored success";
        }
        catch (Exception e)
        {
            debugtext.text = e.ToString();
        }
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

            float forceX = UnityEngine.Random.Range(-xzForce, xzForce);
            float forceZ = UnityEngine.Random.Range(-xzForce, xzForce);
            float forceY = UnityEngine.Random.Range(1, yForce);

            coin.GetComponent<Rigidbody>().AddForce(new Vector3( forceX, forceY, forceZ), ForceMode.Impulse);
            coins.Add(coin);

            yield return new WaitForSeconds(timeBetweenSpawn);
        }
    }



}
