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
        }
        if (transform.name == "Button_Moter_3")
        {
            mqttManager.currentButton = "button3";
            mqttManager.currentButtonState = mqttManager.SendOrder(mqttManager.Button_3_State);
            mqttManager.SendPublishButtonData(mqttManager.currentButton, mqttManager.currentButtonState);
        }
        if (transform.name == "Button_Moter_4")
        {
            mqttManager.currentButton = "button4";
            mqttManager.currentButtonState = mqttManager.SendOrder(mqttManager.Button_4_State);
            mqttManager.SendPublishButtonData(mqttManager.currentButton, mqttManager.currentButtonState);
        }
        if (transform.name == "Button_Power")
        {
            mqttManager.currentButton = "buttonPower";
            mqttManager.currentButtonState = mqttManager.SendOrder(mqttManager.PowerButtonState);
            mqttManager.SendPublishButtonData(mqttManager.currentButton, mqttManager.currentButtonState);
        }    
    }
    /*
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
     * */
}
