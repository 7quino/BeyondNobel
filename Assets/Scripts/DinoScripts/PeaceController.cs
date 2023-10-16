using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class PeaceController : MonoBehaviour {
    [SerializeField] private GameObject _nuke;
    [SerializeField] private List<Transform> _nukeSpawnPoints;
    private bool shouldRun = false;

    public Transform _limiter;
    
    
    public void Open() {
        shouldRun = true;
        StartCoroutine(Tick());
    }

    public void Close() {
        shouldRun = false;
        StopCoroutine(Tick());
    }
    
    IEnumerator Tick() {
        if (!shouldRun) return;

        while (true) {
            yield return new WaitForSeconds(Random.Range(1, 5));
            var randomSpawnNumber = Random.Range(
                0, 
                _nukeSpawnPoints.Count
                );
            Instantiate(
                _nuke, 
                _nukeSpawnPoints[randomSpawnNumber].position,
                _nukeSpawnPoints[randomSpawnNumber].rotation
                );
        }
        yield return null;
    }
}
