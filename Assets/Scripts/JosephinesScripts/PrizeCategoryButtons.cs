using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PrizeCategoryButtons : MonoBehaviour
{
    public static PrizeCategoryButtons instance;
    public List<PrizeCategoryButton> prizeCategoryButtons = new List<PrizeCategoryButton>();
    [HideInInspector] public AudioSource audioSource;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        foreach (var priceCategoryButton in prizeCategoryButtons)
        {
            priceCategoryButton.button.onClick.AddListener(() => OnPriceButtonClicked(priceCategoryButton));
            priceCategoryButton.audioButton.button.onClick.AddListener(() => OnAudioButtonClicked(priceCategoryButton.audioButton));
            priceCategoryButton.InactivateButton(audioSource);
        }

        UiManager.instance.onHidePrizeButtons.AddListener(TurnOffAudioButtons);
        UiManager.instance.onPhotoButtonPressed.AddListener(TurnOffAudioButtons);
    }


    public void OnPriceButtonClicked(PrizeCategoryButton prizeCategoryButton)
    {
        if (prizeCategoryButton.isActive)
        {
            prizeCategoryButton.InactivateButton(audioSource);
        }
        else
        {
            prizeCategoryButton.ActivateButton();
        }
    }

    public void OnAudioButtonClicked(AudioButton audioButton)
    {
        if (audioButton.audioClipCurrentLanguage == null)
        {
            audioButton.SetLanguageClip(audioSource);
        }

        if (audioButton.isActive)
        {
            audioButton.InActivateButton(audioSource);
        }
        else
        {
            TurnOffAudioButtons();
            audioButton.ActivateButton(audioSource);
        }
    }

    public void TurnOffAudioButtons()
    {
        foreach (var button in prizeCategoryButtons)
        {
            button.audioButton.InActivateButton(audioSource);
        }
    }
}


[System.Serializable]
public class PrizeCategoryButton
{
    [Header("Prize button")]
    public string prizeCategory;
    public Button button;
    public GameObject buttonActiveObject;
    public GameObject buttonInactiveObject;
    public AudioButton audioButton;
    [HideInInspector] public bool isActive = true;

    public UnityEvent onButtonClickEnable = new UnityEvent();
    public UnityEvent onButtonClickDisable = new UnityEvent();

    public void InactivateButton(AudioSource audioSource)
    {
        audioButton.InActivateButton(audioSource);
        buttonActiveObject.SetActive(false);
        buttonInactiveObject.SetActive(true);
        onButtonClickDisable.Invoke();
        isActive = false;
    }

    public void ActivateButton()
    {
        buttonActiveObject.SetActive(true);
        buttonInactiveObject.SetActive(false);
        onButtonClickEnable.Invoke();
        isActive = true;
    }
}

[System.Serializable]
public class AudioButton
{
    [Header("Audio button")]
    public Button button;
    public GameObject audioButtonActiveObject;
    public GameObject audioButtonInactiveObject;
    public AudioClip audioClipEn;
    public AudioClip audioClipSv;
    [HideInInspector] public bool isActive = false;
    [HideInInspector] public AudioClip audioClipCurrentLanguage = null;

    public void SetLanguageClip(AudioSource audioSource)
    {
        if (audioClipCurrentLanguage != null) return;

        audioClipCurrentLanguage = LanguageManager.instance._localeID == 0 ? audioClipEn : audioClipSv;
    }

    public void ActivateButton(AudioSource audioSource)
    {
        isActive = true;
        audioButtonActiveObject.SetActive(true);
        audioButtonInactiveObject.SetActive(false);

        audioSource.clip = audioClipCurrentLanguage;
        audioSource.Play();
    }

    public void InActivateButton(AudioSource audioSource)
    {
        isActive = false;
        audioButtonActiveObject.SetActive(false);
        audioButtonInactiveObject.SetActive(true);

        if (audioSource.clip = audioClipCurrentLanguage)
        {
            audioSource.Stop();
        }
    }
}
