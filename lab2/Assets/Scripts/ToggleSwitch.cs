using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
/*
public class ToggleSwitch : MonoBehaviour, IPointerDownHandler 
{
    public bool isOn;
    public RectTransform toggleIndicator;
    public Image backgroundImage;
    public Color onColor;
    public Color offColor;
    public float offX;
    public float onX;
    public float tweenTime = 0.25f;

    public delegate void ValueChanged(bool value);
    public event ValueChanged valueChanged;


    // Start is called before the first frame update
    void Start()
    {
        offX = toggleIndicator.anchoredPosition.x;
        onX = backgroundImage.rectTransform.rect.width - toggleIndicator.rect.width + toggleIndicator.anchoredPosition.x;
    }

    private void OnEnable() {
        Toggle(isOn);
    }

    private void Toggle(bool value) {
        if (value != isOn) {
            isOn = value;

            ToggleColor(isOn);
            MoveIndicator(isOn);

            if (valueChanged != null) {
                valueChanged(isOn);
            }
        }
    }

    private void ToggleColor(bool value) {
        backgroundImage.DOColor(value ? onColor : offColor, tweenTime);
    }

    private void MoveIndicator(bool value) {
        toggleIndicator.DOAnchorPosX(value ? onX : offX, tweenTime);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData) {
        Toggle(!isOn);
    }
}
*/

public class ToggleSwitch : MonoBehaviour
{
    public Toggle toggle;
    public RectTransform toggleIndicator;
    public Image backgroundImage;
    public Color onColor;
    public Color offColor;
    public float offX;
    public float onX;
    public float tweenTime = 0.25f;

    // Start is called before the first frame update
    void Awake()
    {
        offX = toggleIndicator.anchoredPosition.x;
        onX = backgroundImage.rectTransform.rect.width - toggleIndicator.rect.width + offX;

        toggle.onValueChanged.AddListener(OnSwitch);

        if (toggle.isOn) {
            OnSwitch(true);
        }
    }

    void OnSwitch(bool on) {
        ToggleColor(on);
        MoveIndicator(on);
        Debug.Log(string.Format("OnSwitch Toggle: {0}", on));
    }

    private void ToggleColor(bool value) {
        backgroundImage.DOColor(value ? onColor : offColor, tweenTime);
    }

    private void MoveIndicator(bool value) {
        toggleIndicator.DOAnchorPosX(value ? onX : offX, tweenTime);
    }

    void OnDestroy() {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }

}
