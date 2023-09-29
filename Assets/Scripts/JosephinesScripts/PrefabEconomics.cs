using Google.XR.ARCoreExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PrefabEconomics : MonoBehaviour
{
    [Header("AR anchor")]
    [SerializeField] AREarthManager earthManager;
    [SerializeField] ARAnchorManager anchorManager;
    [SerializeField] double latitude;
    [SerializeField] double longitude;
    [SerializeField] double altitude = 38;
    [SerializeField] Quaternion quaternion;
    [SerializeField] GameObject anchorPrefab;
    bool coinHolderAnchored = false;


    [Header("Prefab behaviour")]
    [SerializeField] GameObject coinPrefab;
    [SerializeField] int numberCoins;
    [SerializeField] float timeBetweenSpawn;
    [SerializeField] float xzForce;
    [SerializeField] float yForce;

    [SerializeField] Transform debugCoinHolder;
    [SerializeField] bool debugMode;

    Transform coinHolder;
    List<GameObject> coins = new List<GameObject>();
    IEnumerator spawnCoinsRoutine;

    public void PlaceAnchor()
    {
        if (coinHolderAnchored) return;

        var earthTrackingState = earthManager.EarthTrackingState;
        if (earthTrackingState == TrackingState.Tracking)
        {
            var cameraGeospatialPose = earthManager.CameraGeospatialPose;
            //debugtext.text = "\n" + cameraGeospatialPose.Altitude;

            var anchorGeo = ARAnchorManagerExtensions.AddAnchor(
                    anchorManager,
                    latitude,
                    longitude,
                    cameraGeospatialPose.Altitude,
                    quaternion);

            var anchoredAsset = Instantiate(anchorPrefab, anchorGeo.transform);
            anchoredAsset.transform.position = anchorGeo.transform.position;
            coinHolder = GameObject.Find("CoinHolder").GetComponent<Transform>();

            coinHolderAnchored = true;
        }
    }



    public void OnButtonClickActivate()
    {
        if (!coinHolderAnchored && !debugMode)
        { 
            return;
        }
        else if (debugMode)
        {
            coinHolder = debugCoinHolder;
        }

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

            float forceX = UnityEngine.Random.Range(-xzForce, xzForce);
            float forceZ = UnityEngine.Random.Range(-xzForce, xzForce);
            float forceY = UnityEngine.Random.Range(1, yForce);

            coin.GetComponent<Rigidbody>().AddForce(new Vector3( forceX, forceY, forceZ), ForceMode.Impulse);
            coins.Add(coin);

            yield return new WaitForSeconds(timeBetweenSpawn);
        }
    }
}
