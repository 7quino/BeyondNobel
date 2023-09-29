using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaceController : MonoBehaviour {
    private Camera _camera;
    private bool isRunning = false;

    private void OnEnable() {
        PrizeCategoryButtons.instance.prizeCategoryButtons[5].onButtonClickEnable.AddListener(Open);
        PrizeCategoryButtons.instance.prizeCategoryButtons[5].onButtonClickDisable.AddListener(Close);
    }

    private void OnDisable() {
        PrizeCategoryButtons.instance.prizeCategoryButtons[5].onButtonClickEnable.RemoveListener(Open);
        PrizeCategoryButtons.instance.prizeCategoryButtons[5].onButtonClickDisable.RemoveListener(Close);
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
