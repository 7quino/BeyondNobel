using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PrizeCategoryButtons : MonoBehaviour
{
    public static PrizeCategoryButtons instance;
    public List<PrizeCategoryButton> prizeCategoryButtons = new List<PrizeCategoryButton>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach (var priceCategoryButton in prizeCategoryButtons)
        {
            priceCategoryButton.button.onClick.AddListener(() => OnPriceButtonClicked(priceCategoryButton));
            priceCategoryButton.InactivateButton();
        }
    }


    public void OnPriceButtonClicked(PrizeCategoryButton prizeCategoryButton)
    {
        if (prizeCategoryButton.isActive)
        {
            prizeCategoryButton.InactivateButton();
        }
        else
        {
            prizeCategoryButton.ActivateButton();
        }
    }
}


[System.Serializable]
public class PrizeCategoryButton
{
    public string prizeCategory;
    public Button button;
    public GameObject buttonActiveObject;
    public GameObject buttonInactiveObject;
    [HideInInspector] public bool isActive = true;

    public UnityEvent onButtonClickEnable = new UnityEvent();
    public UnityEvent onButtonClickDisable = new UnityEvent();

    public void InactivateButton()
    {
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
