using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Events;
using UnityEngine.UI;
using static NativeGallery;
using TMPro;
using System.IO;
using System;
using UnityEngine.Localization;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    public UnityEvent<bool> onPrivacyPromptIsOk = new UnityEvent<bool>();
    public UnityEvent onGameStart = new UnityEvent();
    public UnityEvent onTurnOffAudioStories = new UnityEvent();
    public UnityEvent onHidePrizeButtons = new UnityEvent();
    public UnityEvent onPhotoButtonPressed = new UnityEvent();
    [HideInInspector] public UnityEvent<string> onDebugMessage = new UnityEvent<string>();

    [Header("AR Components")]
    [SerializeField] ARSessionOrigin SessionOrigin;
    [SerializeField] ARSession Session;
    [SerializeField] ARCoreExtensions ARCoreExtensions;

    [Header("UI Elements")]
    [SerializeField] GameObject UICanvas;
    [SerializeField] GameObject introCanvas;
    [SerializeField] GameObject privacyPromptCanvas;
    [SerializeField] GameObject instructionsCanvas;
    [SerializeField] GameObject popUpMessageExampleButtons;
    [SerializeField] GameObject arViewCanvas;
    public GameObject vpsCheckCanvas;
    [SerializeField] GameObject ScreenShotCanvas;

    //LocationServiceMessage
    [SerializeField] GameObject locationServiceMessageCanvas;
    bool privacyPromptOkay;
    bool locationServiceSuccess;
    bool locationServiceFailure;
    bool _locationServiceIsFinnished = false;

    [Header("Onboarding Sequance")]
    [SerializeField] GameObject RunOnboardingAgainQuestion;
    [SerializeField] GameObject onboardingCanvas;
    [SerializeField] List<GameObject> onboardingSequence = new List<GameObject>();
    [SerializeField] GameObject saveButton;
    [SerializeField] GameObject shareButton;
    [SerializeField] GameObject throwButton;
    [SerializeField] int onboardingIndex = 0;
    GameObject pressedPrizeButton = null;
    //[SerializeField] SavedPlayerPrefs soPlayerPrefs;
    //bool onboardingHasRun = false;


    [Header("Info Button")]
    [SerializeField] GameObject infoCanvas;

    [Header("Audio Button")]
    [SerializeField] GameObject audioOnIcon;
    [SerializeField] GameObject audioOffIcon;
    public bool audioIsOn = true;

    [Header("Visibility Button")]
    [SerializeField] GameObject bottomButtons;
    [SerializeField] GameObject visibleIcon;
    [SerializeField] GameObject invisibleIcon;

    [Header("Photo Button")]
    [SerializeField] Image screenShotImage;
    Texture2D _lastScreenShotTexture;

    [Header("Save Button")]
    bool _imageSaved = false;

    [Header("PopUp Message")]
    [SerializeField] GameObject popUpMessage;
    [SerializeField] TextMeshProUGUI stringMessageTmpro;
    [SerializeField] const float pupUpTime = 2f;

    [Header("Localized strings")]
    [SerializeField] LocalizedString localizedStringLocationSuccess;
    [SerializeField] LocalizedString localizedStringLocationError;
    [SerializeField] LocalizedString localizedImageSavedSuccess;
    [SerializeField] LocalizedString localizedImageSavedError;
    [SerializeField] LocalizedString localizedImageSavedDone;
    [SerializeField] LocalizedString localizedStringUseEarphones;

    [SerializeField] const float introTime = 3f;
    const string _hasDisplayedPrivacyPromptKey = "HasDisplayedGeospatialPrivacyPrompt";

    
    public void OnOKClicked()
    {
        PlayerPrefs.SetInt(_hasDisplayedPrivacyPromptKey, 1);
        PlayerPrefs.Save();
        privacyPromptOkay = true;
        onPrivacyPromptIsOk.Invoke(true);

        SwitchToInstructions(true);
    }

    public void OnLearnMoreClicked()
    {
        Application.OpenURL("https://developers.google.com/ar/data-privacy");
    }

    public void OnClickedExampleButtons()
    {
        StartCoroutine(ExampleMessageShow());
    }

    IEnumerator ExampleMessageShow()
    {
        popUpMessageExampleButtons.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        popUpMessageExampleButtons.SetActive(false);
    }

    public void OnGetStartedClicked()
    {
        arViewCanvas.SetActive(true);
        ScreenSettingsManager.Instance.PanelSafeArea(arViewCanvas.GetComponent<RectTransform>());
        locationServiceMessageCanvas.SetActive(true);
        ScreenSettingsManager.Instance.PanelSafeArea(locationServiceMessageCanvas.GetComponent<RectTransform>());

        instructionsCanvas.SetActive(false);
        onGameStart.Invoke();

#if UNITY_EDITOR
        locationServiceMessageCanvas.SetActive(false);
#endif

        OnboardingSequence();
    }

    public void OnLocationServiceFinnished()
    {

        if (!privacyPromptOkay) return;
        if (!locationServiceSuccess && !locationServiceFailure) return;
        
        locationServiceMessageCanvas.SetActive(false);

        //if (_locationServiceIsFinnished) return;
        //_locationServiceIsFinnished = true;
        //localizedStringUseEarphones.RefreshString();
        //if (locationServiceSuccess) localizedStringLocationSuccess.RefreshString();
        //if (locationServiceFailure) localizedImageSavedError.RefreshString();
    }

    public void OnboardingSequence(GameObject thisObject = null)
    {
        /*
        if (onboardingHasRun) return;
        if (soPlayerPrefs.onboardingHasRun)
        {
            onboardingCanvas.SetActive(true);
            RunOnboardingAgainQuestion.SetActive(true);
            return;
        }
        */

        if (onboardingIndex == onboardingSequence.Count)
        {
            onboardingCanvas.SetActive(false);
            if (!audioIsOn) OnAudioButtonPressed();
            //soPlayerPrefs.onboardingHasRun = true;
            //onboardingHasRun = true;
            return;
        }

        if (onboardingIndex == 0) onboardingCanvas.SetActive(true);
        if (onboardingIndex == 1) pressedPrizeButton = thisObject;
        if (onboardingIndex == 2 && thisObject != pressedPrizeButton) return;

        if (onboardingIndex != 0) onboardingSequence[onboardingIndex - 1].SetActive(false);
        if (onboardingIndex != onboardingSequence.Count) onboardingSequence[onboardingIndex].SetActive(true);
        if (onboardingIndex == 6 && thisObject == throwButton)
        {
            onboardingCanvas.SetActive(false);
            if (!audioIsOn) OnAudioButtonPressed();
            //soPlayerPrefs.onboardingHasRun = true;
            //onboardingHasRun = true;
        }

        onboardingIndex++;
    }

    /*
    public void PlayOnboardingAgainQuestion(bool answer)
    {
        RunOnboardingAgainQuestion.SetActive(false);
        onboardingCanvas.SetActive(false);

        if (answer)
        {
            soPlayerPrefs.onboardingHasRun = false;
            OnboardingSequence();
        }
        else
        {
            onboardingHasRun = true;
        }
    }
    */

    public void OnContinueClicked()
    {
        vpsCheckCanvas.SetActive(false);
    }

    public void OnInfoButtonShowClicked()
    {
        infoCanvas.SetActive(true);
        ScreenSettingsManager.Instance.PanelSafeArea(infoCanvas.GetComponent<RectTransform>());
        if (!_locationServiceIsFinnished) locationServiceMessageCanvas.SetActive(false);
    }

    public void OnInfoBubbleHideClicked()
    {
        infoCanvas.SetActive(false);
        if (!_locationServiceIsFinnished) locationServiceMessageCanvas.SetActive(true);
    }

    public void OnAudioButtonPressed()
    {
        audioIsOn = !audioIsOn;
        audioOnIcon.SetActive(audioIsOn);
        audioOffIcon.SetActive(!audioIsOn);

        if (!audioIsOn) onTurnOffAudioStories.Invoke();
    }

    public void OnVisibilityButtonClicked()
    {
        if (bottomButtons.activeSelf)
        {
            bottomButtons.SetActive(false);
            visibleIcon.SetActive(true);
            invisibleIcon.SetActive(false);
            onHidePrizeButtons.Invoke();
        }
        else
        {
            bottomButtons.SetActive(true);
            visibleIcon.SetActive(false);
            invisibleIcon.SetActive(true);
        }
    }

    public void OnCameraButton()
    {
        onPhotoButtonPressed.Invoke();
        StartCoroutine(CaptureScreenShot());
    }

    public void OnSaveClicked()
    {
        if (_lastScreenShotTexture == null) return;

        if (_imageSaved)
        {
            localizedImageSavedDone.RefreshString();
            return;
        }

        byte[] mediaBytes = _lastScreenShotTexture.EncodeToPNG();

#if UNITY_EDITOR
        System.IO.File.WriteAllBytes(Application.dataPath + "/screenshot.png", mediaBytes);
#endif
        NativeGallery.Permission permission = SaveImageToGallery(_lastScreenShotTexture, "NobelAR", Time.time.ToString());
        onDebugMessage.Invoke("Save screenshot: " + permission.ToString());

        _ = permission == Permission.Granted ? localizedImageSavedSuccess.RefreshString() : localizedImageSavedError.RefreshString();

        if (permission == Permission.Granted) _imageSaved = true;
        _imageSaved = true;


    }

    public void OnShareClicked()
    {
        if (_lastScreenShotTexture == null) return;
        StartCoroutine(ShareScreenShot());
    }

    public void OnThrowAwayClicked()
    {
        QuitScreenShotMode();
    }


    void OnEnable()
    {
        introCanvas.SetActive(true);
        onboardingCanvas.SetActive(false);
        popUpMessage.SetActive(false);
        privacyPromptCanvas.SetActive(false);
        arViewCanvas.SetActive(false);
        vpsCheckCanvas.SetActive(false);
        infoCanvas.SetActive(false);
        ScreenShotCanvas.SetActive(false);

        //SwitchToInstructions(PlayerPrefs.HasKey(_hasDisplayedPrivacyPromptKey));
    }

    void Awake()
    {
        instance = this;
        
    }

    void Start()
    {
        //Checkings for find location message
        CheckLocationService.Instance.onLocationServiceSuccess.AddListener(() => locationServiceSuccess = true);
        CheckLocationService.Instance.onLocationServiceError.AddListener(() => locationServiceFailure = true);


        //Localized strings
        localizedStringLocationSuccess.StringChanged += ShowMessage;
        localizedStringLocationError.StringChanged += ShowMessage;
        localizedImageSavedSuccess.StringChanged += ShowMessage;
        localizedImageSavedError.StringChanged += ShowMessage;
        localizedImageSavedDone.StringChanged += ShowMessage;
        localizedStringUseEarphones.StringChanged += ShowMessage;

        StartCoroutine(IntroSequence());
    }

    private void Update()
    {
        OnLocationServiceFinnished();
    }

    IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(introTime);

        introCanvas.SetActive(false);
        privacyPromptCanvas.SetActive(true);
        ScreenSettingsManager.Instance.PanelSafeArea(privacyPromptCanvas.GetComponent<RectTransform>());
        instructionsCanvas.SetActive(true);
        ScreenSettingsManager.Instance.PanelSafeArea(instructionsCanvas.GetComponent<RectTransform>());
    }

    void SwitchToInstructions(bool enable)
    {
        SessionOrigin.gameObject.SetActive(enable);
        Session.gameObject.SetActive(enable);
        ARCoreExtensions.gameObject.SetActive(enable);

        privacyPromptCanvas.SetActive(!enable);
        vpsCheckCanvas.SetActive(false);
        if (enable)
        {
            CheckARSession.Instance.CheckAvilability();
        }
    }

    IEnumerator CaptureScreenShot()
    {
        _imageSaved = false;
        UICanvas.SetActive(false);
        if (!_locationServiceIsFinnished) locationServiceMessageCanvas.SetActive(false);

        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;
        Texture2D screenShotTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, width, height);
        screenShotTexture.ReadPixels(rect, 0, 0);
        screenShotTexture.Apply();
        _lastScreenShotTexture = screenShotTexture;

        UICanvas.SetActive(true);
        ScreenShotCanvas.SetActive(true);
        Rect rec = new Rect(0, 0, screenShotTexture.width, screenShotTexture.height);
        Sprite.Create(screenShotTexture, rec, new Vector2(0, 0), 1);
        screenShotImage.sprite = Sprite.Create(screenShotTexture, rec, new Vector2(0, 0), .01f);
    }

    IEnumerator ShareScreenShot()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = _lastScreenShotTexture;
        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        new NativeShare().AddFile(filePath)
            .SetSubject("Subject goes here").SetText(LanguageManager.instance._localeID == 0 ? "Greetings from Beyond Nobel!" : "H�lsningar fr�n Beyond Nobel!").SetUrl("https://github.com/yasirkula/UnityNativeShare")
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();

        /*
        if (NativeShare.TargetExists("com.whatsapp"))
            new NativeShare().AddFile(filePath).AddTarget("com.whatsapp").Share();
        if (NativeShare.TargetExists("com.facebook"))
            new NativeShare().AddFile(filePath).AddTarget("com.facebook").Share();
        if (NativeShare.TargetExists("com.snapshat"))
            new NativeShare().AddFile(filePath).AddTarget("com.facebook").Share();
        if (NativeShare.TargetExists("com.snapshat"))
            new NativeShare().AddFile(filePath).AddTarget("com.instagram").Share();
        if (NativeShare.TargetExists("com.snapshat"))
            new NativeShare().AddFile(filePath).AddTarget("com.tiktok").Share();
        */
    }

    void QuitScreenShotMode()
    {
        arViewCanvas.SetActive(true);
        if (!_locationServiceIsFinnished) locationServiceMessageCanvas.SetActive(true);
        if (popUpMessage.activeInHierarchy) popUpMessage.SetActive(false);
        ScreenShotCanvas.SetActive(false);
        _lastScreenShotTexture = null;
        _imageSaved = false;
    }


    public void ShowMessage(string message)
    {
        StartCoroutine(PopUpMessage(message));
    }
    
    
    IEnumerator PopUpMessage(string message, float time = pupUpTime)
    {
        popUpMessage.SetActive(true);
        stringMessageTmpro.text = message;

        yield return new WaitForSeconds(time);

        stringMessageTmpro.text = string.Empty;
        popUpMessage.SetActive(false);
    }
}
    