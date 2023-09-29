using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaceController : MonoBehaviour {
    private Camera _camera;
    private bool isRunning = false;

    private void OnEnable() {
        
    }

    private void OnDisable() {
        
    }

    public void Open() {
        StartCoroutine(SetReferences());
    }

    public void Close() {
        isRunning = false;
    }
    
    IEnumerator SetReferences() {
        _camera = Camera.main;
        
        yield return new WaitUntil(() => {
            return _camera != null;
        });

        StartCoroutine(StartGame());
    }
    

    IEnumerator StartGame() {
        isRunning = true;
        yield return null;
    }

    IEnumerator GameLoop() {
        while (isRunning) {
            
        }

        yield return null;
    }
}
