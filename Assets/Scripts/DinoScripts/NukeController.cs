using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NukeController : MonoBehaviour {
    [SerializeField] private GameObject _fireworkPrefab;
    [SerializeField] private PeaceController _peaceController;
    

    private void OnEnable() {
        StartCoroutine(Fly());
    }

    IEnumerator Fly() {
        var randomLimiterOffset = Random.Range(0, 5);
        var limiter = _peaceController._limiter;
        transform.Translate(transform.forward);
        yield return new WaitWhile(
            () => transform.position.y < 
                  limiter.position.y 
                  + randomLimiterOffset
        );
        Instantiate(
            _fireworkPrefab, 
            transform.position, 
            Quaternion.identity
            ); 
        Destroy(gameObject, 0.5f);
    }
}
