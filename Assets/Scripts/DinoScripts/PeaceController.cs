using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class PeaceController : MonoBehaviour {
    [SerializeField] private GameObject _nuke;
    [SerializeField] private List<Transform> _nukeSpawnPoints;

    public Transform _limiter;
    
    
    public void Open() {
        StartCoroutine(Tick());
    }

    public void Close() {
        StopCoroutine(Tick());
    }
    
    IEnumerator Tick() {
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
