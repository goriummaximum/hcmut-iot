using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;

namespace MyDashboard
{
    public class StatusData
    {
        public string temperature { get; set; }
        public string humidity { get; set; }
    }

    public class ControlData
    {
        public string device { get; set; }
        public string status { get; set; }

    }

    public class MQTTManager : M2MqttUnityClient
    {
        private string student_id = "1852161";
        public List<string> topics = new List<string>();
        private List<string> eventMessages = new List<string>();
        public DashboardManager dashboard_manager;

        [SerializeField]
        public StatusData status_data;
        [SerializeField]
        public ControlData controlLedData;
        [SerializeField]
        public ControlData controlPumpData;
        public void PublishLed()
        {
            controlLedData = dashboard_manager.GetControlLedData();
            string msg_control = JsonConvert.SerializeObject(controlLedData);
            client.Publish(topics[1], System.Text.Encoding.UTF8.GetBytes(msg_control), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("publish control led");
        }

        public void PublishPump()
        {
            controlPumpData = dashboard_manager.GetControlPumpData();
            string msg_control = JsonConvert.SerializeObject(controlPumpData);
            client.Publish(topics[2], System.Text.Encoding.UTF8.GetBytes(msg_control), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("publish control pump");
        }

	    public void UpdateBeforeConnect()
        {
            this.brokerAddress = dashboard_manager.addressInputField.text;
            this.brokerPort = 1883;
            this.mqttUserName = dashboard_manager.userInputField.text;
            this.mqttPassword = dashboard_manager.pwdInputField.text;
            topics.Add(string.Format("/bkiot/{0}/status", student_id));
            topics.Add(string.Format("/bkiot/{0}/led", student_id));
            topics.Add(string.Format("/bkiot/{0}/pump", student_id));
            this.Connect();
        }
       
        public void SetEncrypted(bool isEncrypted)
        {
            this.isEncrypted = isEncrypted;
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
            dashboard_manager.updateLoginStatus(string.Format("connecting to {0}:{1}...", this.brokerAddress, this.brokerPort.ToString()));
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            dashboard_manager.updateLoginStatus("connected");
            SubscribeTopics();
            dashboard_manager.SwitchLayer();
        }

        protected override void SubscribeTopics()
        {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                    Debug.Log(string.Format("subscribed topic: {0}", topic));
                }
            }
        }

        protected override void UnsubscribeTopics()
        {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Unsubscribe(new string[] { topic });
                }
            }

        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            dashboard_manager.updateLoginStatus("connection failed");
        }

        protected override void OnDisconnected()
        {

        }

        protected override void OnConnectionLost()
        {
            dashboard_manager.updateLoginStatus("connection lost");
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);

            Debug.Log("Received: " + msg);
            if (topic == topics[0])
                ProcessMessageStatus(msg);
            else if (topic == topics[1])
                ProcessMessageLedControl(msg);
            else if (topic == topics[2])
                ProcessMessagePumpControl(msg);
        }

        private void ProcessMessageStatus(string msg)
        {
            status_data = JsonConvert.DeserializeObject<StatusData>(msg);
            status_data.temperature = Math.Round(float.Parse(status_data.temperature), 2).ToString();
            status_data.humidity = Math.Round(float.Parse(status_data.humidity), 2).ToString();
            dashboard_manager.updateStatus(status_data);
        }

        private void ProcessMessageLedControl(string msg)
        {
            controlLedData = JsonConvert.DeserializeObject<ControlData>(msg);
            dashboard_manager.UpdateLedControlSwitch(controlLedData);
        }

        private void ProcessMessagePumpControl(string msg)
        {
            controlPumpData = JsonConvert.DeserializeObject<ControlData>(msg);
            dashboard_manager.UpdatePumpControlSwitch(controlPumpData);
        }

        protected override void Update()
        {
            base.Update();
        }

        private void OnDestroy()
        {
            Disconnect();
        }
    }
}
