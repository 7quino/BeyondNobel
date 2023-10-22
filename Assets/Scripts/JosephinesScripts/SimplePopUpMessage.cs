using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class SimplePopUpMessage : MonoBehaviour
{
    [SerializeField] LocalizedString localizedStringUserDirection;

    private void Start()
    {
        localizedStringUserDirection.StringChanged += UpdateText;
    }

    public void ShowPopup()
    {
        if (localizedStringUserDirection != null)
        {
            localizedStringUserDirection.RefreshString();
        }
    }

    void UpdateText(string message)
    {
        UiManager.instance.ShowMessage(message);
    }
}
