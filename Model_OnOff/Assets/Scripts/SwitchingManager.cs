using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아두이노로 온값을 스위칭을 통해 버튼에 전달한다.
/// 버튼을 눌렀을 경우 아두이노로 값을 보낸다.
/// 보낸값을 스위칭을 통해 버튼에 나타낸다.
/// </summary>
public class SwitchingManager : MonoBehaviour
{
    public UILabel onLabel;
    public UILabel offLabel;      
   

    /// <summary>
    /// 서버로 부터 받은 버튼 정보를 보여준다.
    /// </summary>
    public void SetSwitching(bool buttonState)
    {
        onLabel.gameObject.SetActive(buttonState);
        offLabel.gameObject.SetActive(!buttonState);      
    }


    /// <summary>
    /// 버튼 클릭시 서버로 1&0 값을 보낸다.
    /// </summary>
    public void SendSwitchData()
    {
        MqttManager mqttManager = GameObject.Find("UI Root (3D)").GetComponent<MqttManager>();
        if (transform.name == "Button_Power")
            mqttManager.SendPublishButtonData("buttonPower", (mqttManager.PowerButtonState == "1") ? "0" : "1");
        if (transform.name == "Button_Moter_1")
            mqttManager.SendPublishButtonData("button1", (mqttManager.Button_1_State == "1") ? "0" : "1");
        if (transform.name == "Button_Moter_2")
            mqttManager.SendPublishButtonData("button2", (mqttManager.Button_2_State == "1") ? "0" : "1");
        if (transform.name == "Button_Moter_3")
            mqttManager.SendPublishButtonData("button3", (mqttManager.Button_3_State == "1") ? "0" : "1");
        if (transform.name == "Button_Moter_4")
            mqttManager.SendPublishButtonData("button4", (mqttManager.Button_4_State == "1") ? "0" : "1");
    }
}
