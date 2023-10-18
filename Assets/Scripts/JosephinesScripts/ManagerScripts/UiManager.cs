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

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    public UnityEvent<bool> onPrivacyPromptIsOk = new UnityEvent<bool>();
    public UnityEvent onGameStart = new UnityEvent();
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
    [SerializeField] GameObject arViewCanvas;
    public GameObject vpsCheckCanvas;
    [SerializeField] GameObject ScreenShotCanvas;

    //LocationServiceMessage
    [SerializeField] GameObject locationServiceMessageCanvas;
    public TextMeshProUGUI locationServiceMessage;
    bool _locationServiceIsFinnished = false;

    //Info button
    [SerializeField] GameObject infoCanvas;
    [SerializeField] GameObject infoBubble1;
    [SerializeField] GameObject infoBubble2;

    //Visibility button
    [SerializeField] GameObject bottomButtons;
    [SerializeField] GameObject visibleIcon;
    [SerializeField] GameObject invisibleIcon;

    //Camera button
    [SerializeField] Image screenShotImage;
    Texture2D _lastScreenShotTexture;

    //Save button
    bool _imageSaved = false;

    //PopUp Message
    [SerializeField] GameObject popUpMessage;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] const float pupUpTime = 2f;

    //Share button
    //private bool isFocus = false;
    //private bool isProcessing = false;


    [SerializeField] const float introTime = 3f;
    const string _hasDisplayedPrivacyPromptKey = "HasDisplayedGeospatialPrivacyPrompt";
    bool privacyPromptIsOk = false;
    
    public void OnOKClicked()
    {
        PlayerPrefs.SetInt(_hasDisplayedPrivacyPromptKey, 1);
        PlayerPrefs.Save();
        SwitchToInstructions(true);
    }

    public void OnLearnMoreClicked()
    {
        Application.OpenURL("https://developers.google.com/ar/data-privacy");
    }

    public void OnGetStartedClicked()
    {
        arViewCanvas.SetActive(true);
        ScreenSettingsManager.Instance.PanelSafeArea(arViewCanvas.GetComponent<RectTransform>());
        locationServiceMessageCanvas.SetActive(true);
        ScreenSettingsManager.Instance.PanelSafeArea(locationServiceMessageCanvas.GetComponent<RectTransform>());

        instructionsCanvas.SetActive(false);
        onGameStart.Invoke();
    }

    public void OnLocationServiceFinniched(string message)
    {
        if (_locationServiceIsFinnished) return;

        locationServiceMessageCanvas.SetActive(false);
        _locationServiceIsFinnished = true;
        StartCoroutine(PopUpMessage(message, 2.0f));
    }

    public void OnContinueClicked()
    {
        vpsCheckCanvas.SetActive(false);
    }

    public void OnInfoButtonClicked()
    {
        infoCanvas.SetActive(true);
        ScreenSettingsManager.Instance.PanelSafeArea(infoCanvas.GetComponent<RectTransform>());
        infoBubble1.SetActive(true);
    }

    public void OnInfoBubble1Clicked()
    {
        infoBubble1.SetActive(false);
        infoBubble2.SetActive(true);
    }

    public void OnInfoBubble2Clicked()
    {
        infoBubble2.SetActive(false);
        infoCanvas.SetActive(false);
    }

    public void OnVisibilityButtonClicked()
    {
        if (bottomButtons.activeSelf)
        {
            bottomButtons.SetActive(false);
            visibleIcon.SetActive(true);
            invisibleIcon.SetActive(false);
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
        StartCoroutine(CaptureScreenShot());
    }

    public void OnSaveClicked()
    {
        if (_lastScreenShotTexture == null) return;

        if (_imageSaved)
        {
            StartCoroutine(PopUpMessage("Image already saved!"));
            return;
        }

        byte[] mediaBytes = _lastScreenShotTexture.EncodeToPNG();

#if UNITY_EDITOR
        System.IO.File.WriteAllBytes(Application.dataPath + "/screenshot.png", mediaBytes);
#endif
        NativeGallery.Permission permission = SaveImageToGallery(_lastScreenShotTexture, "NobelAR", Time.time.ToString());
        onDebugMessage.Invoke("Save screenshot: " + permission.ToString());

        string message = permission == Permission.Granted ? "Image saved to camera roll!" : "Image save failed!";
        StartCoroutine(PopUpMessage(message));
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

        popUpMessage.SetActive(false);
        privacyPromptCanvas.SetActive(false);
        arViewCanvas.SetActive(false);
        vpsCheckCanvas.SetActive(false);
        infoBubble1.SetActive(false);
        infoBubble2.SetActive(false);
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
        CheckLocationService.Instance.onLocationServiceSuccess.AddListener(() => OnLocationServiceFinniched("Location found!"));
        CheckLocationService.Instance.onLocationServiceError.AddListener(() => OnLocationServiceFinniched("No location service,\nusing plan B"));
        
        StartCoroutine(IntroSequence());
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
        privacyPromptIsOk = enable;
        onPrivacyPromptIsOk.Invoke(enable);
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
            .SetSubject("Subject goes here").SetText("Hello world!").SetUrl("https://github.com/yasirkula/UnityNativeShare")
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();

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
        messageText.text = message;
        

        yield return new WaitForSeconds(time);

        messageText.text = string.Empty;
        popUpMessage.SetActive(false);
    }
}
    