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
    /// 이유: 반대로 보여 줘야 한다.
    /// 끔(0)을 아두이노로 보낸다.(끔버튼을 누를때)  -> 스위칭된 현재 버튼은 반대의 켬상태로 보여진다.(켜진줄 안다)
    /// 켬(1)을 아두이노로 보낸다.(켬버튼을 누를때) -> 스위칭된 현재 버튼은  반대의 꺼짐상태가 된다.(꺼진줄 안다)
    /// 
    /// 아두이노로 부터 0을 다시 받기위해선 0을 보내야 한다.
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
}
