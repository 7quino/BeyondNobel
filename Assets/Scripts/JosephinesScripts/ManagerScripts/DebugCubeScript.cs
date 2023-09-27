using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCubeScript : MonoBehaviour
{
    public void OnDisableButton()
    {
        this.gameObject.SetActive(false);
    }

    public void OnEnableButton()
    {
        this .gameObject.SetActive(true);
    }
}
