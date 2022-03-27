using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MyDashboard
{
    public class DashboardManager : MonoBehaviour
    {
        /// <summary>
        /// Layer Login elements
        /// </summary>
        [SerializeField]
        private CanvasGroup _canvasLayerLogin;
        [SerializeField]
        public InputField addressInputField;
        [SerializeField]
        public InputField userInputField;
        [SerializeField]
        public InputField pwdInputField;
        [SerializeField]
        public Button connectButton;
        [SerializeField]
        public Text loginStatus;
        /// <summary>
        /// Layer Board elements
        /// </summary>
        [SerializeField]
        private CanvasGroup _canvasLayerBoard;
        [SerializeField]
        public Text temperature_value;
        [SerializeField]
        public Text humidity_value;
        [SerializeField]
        public ToggleSwitch controlLedSwitch;
        [SerializeField]
        public ToggleSwitch controlPumpSwitch;


        /// <summary>
        /// General elements
        /// </summary>
        [SerializeField]
        private GameObject btnClose;

        private Tween twenFade;

        void Start() {
            _InitializeLoginInputField();
        }

        public void _InitializeLoginInputField() {
            addressInputField.text = "mqttserver.tk";
            userInputField.text = "bkiot";
            pwdInputField.text = "12345678";
        }

        public void updateLoginStatus(string status) {
            loginStatus.text = status;
        }

        public void updateStatus(StatusData status_data)
        {
            temperature_value.text = string.Format("{0} °C", status_data.temperature);
            humidity_value.text = string.Format("{0} %", status_data.humidity);
        }

        public ControlData GetControlLedData() {
            ControlData controlLedData = new ControlData();;
            controlLedData.device = "LED";
            //toggle: isOn ? OFF : ON
            controlLedData.status = (controlLedSwitch.toggle.isOn ? "OFF" : "ON");
            controlLedSwitch.toggle.interactable = false;
            return controlLedData;
        }

        public ControlData GetControlPumpData() {
            ControlData controlPumpData = new ControlData();
            controlPumpData.device = "PUMP";
            //toggle: isOn ? OFF : ON
            controlPumpData.status = (controlPumpSwitch.toggle.isOn ? "OFF" : "ON");
            controlPumpSwitch.toggle.interactable = false;
            return controlPumpData;
        }

        public void UpdateLedControlSwitch(ControlData controlData)
        {
            controlLedSwitch.toggle.interactable = true;
            if (controlData.status == "ON")
                controlLedSwitch.toggle.isOn = true;
            else
                controlLedSwitch.toggle.isOn = false;
        }

        public void UpdatePumpControlSwitch(ControlData controlData)
        {
            controlPumpSwitch.toggle.interactable = true;
            if (controlData.status == "ON")
                controlPumpSwitch.toggle.isOn = true;
            else
                controlPumpSwitch.toggle.isOn = false;
        }

        public void Fade(CanvasGroup _canvas, float endValue, float duration, TweenCallback onFinish)
        {
            if (twenFade != null)
            {
                twenFade.Kill(false);
            }

            twenFade = _canvas.DOFade(endValue, duration);
            twenFade.onComplete += onFinish;
        }

        public void FadeIn(CanvasGroup _canvas, float duration)
        {
            Fade(_canvas, 1f, duration, () =>
            {
                _canvas.interactable = true;
                _canvas.blocksRaycasts = true;
            });
        }

        public void FadeOut(CanvasGroup _canvas, float duration)
        {
            Fade(_canvas, 0f, duration, () =>
            {
                _canvas.interactable = false;
                _canvas.blocksRaycasts = false;
            });
        }

        IEnumerator _IESwitchLayer()
        {
            if (_canvasLayerLogin.interactable == true)
            {
                FadeOut(_canvasLayerLogin, 0.25f);
                yield return new WaitForSeconds(0.5f);
                FadeIn(_canvasLayerBoard, 0.25f);
            }
            else
            {
                FadeOut(_canvasLayerBoard, 0.25f);
                yield return new WaitForSeconds(0.5f);
                FadeIn(_canvasLayerLogin, 0.25f);
            }
        }

        public void SwitchLayer()
        {
            StartCoroutine(_IESwitchLayer());
        }
    }
}