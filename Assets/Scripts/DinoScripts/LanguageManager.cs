using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.Localization.Settings;


public class LanguageManager : MonoBehaviour {

    private bool active = false;
    
    public void ChangeLocale(int localeID) {
        if (active) return;
        StartCoroutine(SetLocale(localeID));
    }
    
    IEnumerator SetLocale(int _localeID) {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        active = false;
    }
}
