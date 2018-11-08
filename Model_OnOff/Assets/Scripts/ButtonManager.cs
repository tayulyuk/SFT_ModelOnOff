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
        if (transform.name == "reConnectButton")
            mqttManager.isReConnect = false;
        if (transform.name == "errorButton")
            mqttManager.isError = false;
        if (transform.name == "Button - Exit")
            Application.Quit();
        if (transform.name == "Button_Moter_1")  
            mqttManager.SendPublishButtonData("button1", SendOrder(mqttManager.Button_1_State));
        if (transform.name == "Button_Moter_2")
            mqttManager.SendPublishButtonData("button2", SendOrder(mqttManager.Button_2_State));
        if (transform.name == "Button_Moter_3")
            mqttManager.SendPublishButtonData("button3",  SendOrder(mqttManager.Button_3_State));
        if (transform.name == "Button_Moter_4")
            mqttManager.SendPublishButtonData("button4", SendOrder(mqttManager.Button_4_State));
        if (transform.name == "Button_Power")      
            mqttManager.SendPublishButtonData("buttonPower",  SendOrder(mqttManager.PowerButtonState));      
    }

    /// <summary>
    /// 버튼의 스위칭 명령   0->1   1->0변환 명령.
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    private string SendOrder(string order)
    {
        string v = "";
        if (order == "1")
            v = "0";
        if (order == "0")
            v = "1";
        else
            v = "0";
        return v;
    }

    public void SendAllButtonSetting()
    {
        if (transform.name == "Button_Moter_1")
            SendMessage("SendSwitchData", (GameObject.Find("UI Root (3D)").GetComponent<MqttManager>().Button_1_State == "1") ? false : true);
        if (transform.name == "Button_Moter_2")
            SendMessage("SendSwitchData", (GameObject.Find("UI Root (3D)").GetComponent<MqttManager>().Button_2_State == "1") ? false : true);
        if (transform.name == "Button_Moter_3")
            SendMessage("SendSwitchData", (GameObject.Find("UI Root (3D)").GetComponent<MqttManager>().Button_3_State == "1") ? false : true);
        if (transform.name == "Button_Moter_4")
            SendMessage("SendSwitchData", (GameObject.Find("UI Root (3D)").GetComponent<MqttManager>().Button_4_State == "1") ? false : true);
        if (transform.name == "Button_Power")
            SendMessage("SendSwitchData", (GameObject.Find("UI Root (3D)").GetComponent<MqttManager>().PowerButtonState == "1") ? false : true);
    }
}
