using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public bool pingPong;
    private MqttManager mqttManager;

    void Start()
    {
        mqttManager = GameObject.Find("UI Root (3D)").GetComponent<MqttManager>();
        pingPong = false;
        //SendAllButtonSetting();
    }
    private void OnClick()
    {
       // if (transform.name == "reConnectButton")
      //      mqttManager.isReConnect = false;
        if (transform.name == "loadingButton")
            mqttManager.isLoading = false;
        if (transform.name == "Button - Exit")
            Application.Quit();
        if (transform.name == "Button_Moter_1")
        {
            mqttManager.currentButton = "button1";
            mqttManager.currentButtonState = mqttManager.SendOrder(mqttManager.Button_1_State);
            mqttManager.SendPublishButtonData(mqttManager.currentButton, mqttManager.currentButtonState);

            //정상.  버튼의 상태를 보여주면 된다.  아니라면  다시 보낸다.

            StartCoroutine(mqttManager.ReSendToServer());
        }
        if (transform.name == "Button_Moter_2")
        {
            mqttManager.currentButton = "button2";
            mqttManager.currentButtonState = mqttManager.SendOrder(mqttManager.Button_2_State);
            mqttManager.SendPublishButtonData(mqttManager.currentButton, mqttManager.currentButtonState);
            StartCoroutine(mqttManager.ReSendToServer());
        }
        if (transform.name == "Button_Moter_3")
        {
            mqttManager.currentButton = "button3";
            mqttManager.currentButtonState = mqttManager.SendOrder(mqttManager.Button_3_State);
            mqttManager.SendPublishButtonData(mqttManager.currentButton, mqttManager.currentButtonState);
            StartCoroutine(mqttManager.ReSendToServer());
        }
        if (transform.name == "Button_Moter_4")
        {
            mqttManager.currentButton = "button4";
            mqttManager.currentButtonState = mqttManager.SendOrder(mqttManager.Button_4_State);
            mqttManager.SendPublishButtonData(mqttManager.currentButton, mqttManager.currentButtonState);
            StartCoroutine(mqttManager.ReSendToServer());
        }
        if (transform.name == "Button_Power")
        {
            mqttManager.currentButton = "buttonPower";
            mqttManager.currentButtonState = mqttManager.SendOrder(mqttManager.PowerButtonState);
            mqttManager.SendPublishButtonData(mqttManager.currentButton, mqttManager.currentButtonState);
            StartCoroutine(mqttManager.ReSendToServer());
        }    
    }
}
