using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePopUpMessage : MonoBehaviour
{
    [SerializeField] string userDirection = string.Empty;

    public void ShowPopup()
    {
        if (userDirection != string.Empty)
        {
            UiManager.instance.ShowMessage(userDirection);
        }    
    }
}
