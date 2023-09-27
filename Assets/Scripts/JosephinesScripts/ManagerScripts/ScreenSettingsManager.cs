using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScreenSettingsManager : MonoBehaviour
{
    public static ScreenSettingsManager Instance;
    [HideInInspector] public UnityEvent<string> onDebugMessage = new UnityEvent<string>();

    [SerializeField] List<RectTransform> screens = new List<RectTransform>();

    void Awake()
    {
        Instance = this;

        // Lock screen to portrait.
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.orientation = ScreenOrientation.Portrait;
        Application.targetFrameRate = 60;

        Rect safeArea = Screen.safeArea;
        foreach (var screen in screens)
        {
            PanelSafeArea(screen);
        }
    }

    public void PanelSafeArea(RectTransform panel)
    {
        Rect safeArea = Screen.safeArea;

        // Check for invalid screen startup state on some Samsung devices (see below)
        if (Screen.width > 0 && Screen.height > 0)
        {
            // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            if (anchorMin.x >= 0 && anchorMin.y >= 0 && anchorMax.x >= 0 && anchorMax.y >= 0)
            {
                panel.anchorMin = anchorMin;
                panel.anchorMax = anchorMax;

                onDebugMessage.Invoke("SafeScreenArea: Success");
            }
        }
        else
        {
            onDebugMessage.Invoke("SafeScreenArea: Failure");
        }
    }
}
