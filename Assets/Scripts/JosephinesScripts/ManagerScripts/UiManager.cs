using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Events;
using UnityEngine.UI;
using static NativeGallery;
using TMPro;

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
    private bool isFocus = false;
    private bool isProcessing = false;

    public NativeShare nativeShareManager;


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

        instructionsCanvas.SetActive(false);
        onGameStart.Invoke();
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
            StartCoroutine(PopUpMessage("Image already saved"));
            return;
        }

        byte[] mediaBytes = _lastScreenShotTexture.EncodeToPNG();

        //System.IO.File.WriteAllBytes(Application.dataPath + "/screenshot.png", mediaBytes); //PC
        NativeGallery.Permission permission = SaveImageToGallery(mediaBytes, "NobelAR", Time.time.ToString());
        //NativeGallery.Permission permission2 = SaveImageToGallery(_lastScreenShotTexture, "NobelAR", Time.time.ToString());

        onDebugMessage.Invoke("Save screenshot: " + permission.ToString());

        string message = permission == Permission.Granted ? "Image saved to camera roll" : "Image save failed";
        StartCoroutine(PopUpMessage(message));
        if (permission == Permission.Granted) _imageSaved = true;
    }

    public void OnShareClicked()
    {
        if (_lastScreenShotTexture == null) return;

        nativeShareManager.Share(_lastScreenShotTexture);

        /*
        //ShareImage();
        byte[] bytes = _lastScreenShotTexture.EncodeToPNG();
        string screenshotPath = Application.persistentDataPath + "/screenshot.png";
        File.WriteAllBytes(screenshotPath, bytes);

        StartCoroutine(ShareScreenshotCoroutine(screenshotPath));
        */
    }

    public void OnThrowAwayClicked()
    {
        QuitScreenShotMode();
    }


    void OnEnable()
    {
        introCanvas.SetActive(true);

        privacyPromptCanvas.SetActive(false);
        popUpMessage.SetActive(false);
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

    void QuitScreenShotMode()
    {
        arViewCanvas.SetActive(true);
        ScreenShotCanvas.SetActive(false);
        _lastScreenShotTexture = null;
        _imageSaved = false;
    }

    IEnumerator PopUpMessage(string message)
    {
        messageText.text = message;
        popUpMessage.SetActive(true);

        yield return new WaitForSeconds(pupUpTime);

        messageText.text = string.Empty;
        popUpMessage.SetActive(false);
    }


    


}
