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
        transform.LookAt(_peaceController._limiter);
    }

    private void Update() {
        transform.Translate(Vector3.forward * 100 * Time.deltaTime);
    }

    IEnumerator Fly() {
        var randomLimiterOffset = Random.Range(-10, 10);
        var limiter = _peaceController._limiter;
        yield return new WaitWhile(
            () => transform.position.y < 
                  limiter.position.y 
                  + randomLimiterOffset
        );
        Instantiate(
            _fireworkPrefab, 
            transform.position, 
            Quaternion.RotateTowards(transform.rotation, Camera.main.transform.rotation, 0.1f)
            ); 
        Destroy(gameObject, 0.01f);
    }
}
