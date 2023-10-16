using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEditor.Localization.Editor;
using UnityEngine.Localization.Settings;

public enum Language {English, Swedish} 
public class LanguageManager : MonoBehaviour {
    
    public static LanguageManager instance;
    public Language selectedLanguage;
    private bool active = false;
    private int _localeID;
    private int oldSelectedLanguageState;
    [SerializeField] private LanguageButton _button;

    
    private void Awake()
    {
        instance = this;
    }
    

    IEnumerator Start() {
        oldSelectedLanguageState = (int)selectedLanguage;
        
        yield return LocalizationSettings.InitializationOperation;
        
        switch (LocalizationSettings.SelectedLocale.Identifier.Code) {
            case "en":
                selectedLanguage = Language.English;  
                _button.enText.SetActive(true);
                _button.svText.SetActive(false);
                Debug.Log("Your locale is: English");
                
                break;
            case "sv":
                selectedLanguage = Language.Swedish;
                _button.enText.SetActive(false);
                _button.svText.SetActive(true);
                Debug.Log("Your locale is: Swedish");
                break;
        }

        StartCoroutine(Tick());
    }

    IEnumerator Tick() {
        while (true) {
            if ((int)selectedLanguage != oldSelectedLanguageState) { 
                
                switch ((int)selectedLanguage) {
                    case 0:
                        if (!_button.enText.activeSelf) {
                            _button.enText.SetActive(true);
                            _button.svText.SetActive(false);
                        }
                        break;
                    case 1:
                        if (!_button.svText.activeSelf) {
                            _button.svText.SetActive(true);
                            _button.enText.SetActive(false);
                        }
                        break;
                }
                
                ChangeLocale((int)selectedLanguage);
                oldSelectedLanguageState = (int)selectedLanguage;
            }

            yield return null;
        }
    }

    public void ChangeLocale(int localeID) {
        if (active) return;
        
        StartCoroutine(SetLocale(localeID));
    }
    
    IEnumerator SetLocale(int _localeID) {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        Debug.Log($"Changing locale to: {LocalizationSettings.SelectedLocale.LocaleName}");
        active = false;
    }
}
