using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkController : MonoBehaviour
{
    private void OnEnable() {
        Destroy(gameObject, 5);
    }
}
