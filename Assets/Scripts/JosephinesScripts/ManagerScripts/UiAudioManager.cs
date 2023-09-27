using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiAudioManager : MonoBehaviour
{
    [SerializeField] AudioClip introJingleClip;
    [SerializeField] AudioClip buttonClickClip;
    [SerializeField] List<Button> uiButtons;

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = introJingleClip;
        audioSource.PlayOneShot(introJingleClip);

        foreach (var button in uiButtons)
        {
            button.onClick.AddListener(() => { audioSource.clip = buttonClickClip; audioSource.PlayOneShot(buttonClickClip); });
        }
    }
}
