using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LanguageButton : MonoBehaviour
{
    [SerializeField] GameObject svText;
    [SerializeField] GameObject enText;
    [HideInInspector] public Button languageButton;

    public UnityEvent onEnglish = new UnityEvent();
    public UnityEvent onSwedish = new UnityEvent();

    void Start()
    {
        languageButton = GetComponent<Button>();
        languageButton.onClick.AddListener(OnLanguageButtonClicked);
        svText.SetActive(false);
        enText.SetActive(true);
    }

    void OnLanguageButtonClicked()
    {
        enText.SetActive(!enText.activeSelf);
        svText.SetActive(!svText.activeSelf);

        if (enText.activeInHierarchy)
        {
            onEnglish.Invoke();
        }
        else
        {
            onSwedish.Invoke();
        }
    }
}
