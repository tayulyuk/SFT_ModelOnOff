﻿using UnityEngine;
using System.Collections;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System;

public class MqttManager : MonoBehaviour
{
    private MqttClient client;
   
    /// <summary>
    /// 서버로 부터 받은 버튼들의 상태.
    /// </summary>
    public string PowerButtonState;
    public string Button_1_State;
    public string Button_2_State;
    public string Button_3_State;
    public string Button_4_State;

    /// <summary>
    /// 버튼 최종 정렬하기 위해 한번 실행.
    /// </summary>
    public bool isOne; 

    public GameObject buttonPowerObject;
    public GameObject button1Object;
    public GameObject button2Object;
    public GameObject button3Object;
    public GameObject button4Object;

    public GameObject loadingPopUpObject;
    public GameObject reConnectPopUpObject;
   // public bool isError; // error message 들어오면 팝업 띠워주자.
  //  public bool isReConnect; // 아두이노 wifi통신이 다시 접속했다는 메시지 창. -- 사용자가 불안해 한다.  고민하다 수정.
    public string currentButton;  // 명령 버튼명을 저장후  서버로 부터 받은 번호와 같은지 비교하기 위해 저장.
    public string currentButtonState;// 명령 버튼의 상태를 저장후  서버로 부터 받은 번호와 같은지 비교하기 위해 저장.
    public bool isLoading; // 버튼 누르고 로딩 화면 보여 줄려고.
    public float time; // 로딩이 3초 이상 될때는 화면을 꺼줘라. 다시 누를수 있도록.
    void Start()
    {
        time = 0;
        // create client instance 
        client = new MqttClient(IPAddress.Parse("119.205.235.214"), 1883, false, null);

        // register to message received 
        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

       string clientId = Guid.NewGuid().ToString();
        //string clientId = "siheung_namu_moter";
        client.Connect(clientId);

        // subscribe to the topic "/home/temperature" with QoS 2 
        client.Subscribe(new string[] { "ModelOnOff/result" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

       // SendPublishButtonData("ping", "ping");  시흥에는 없는 부분
        isOne = true;  
    }

    /// <summary>
    /// 버튼 클릭 현재 정보를 전달한다.
    /// </summary>
    /// <param name="topic">버튼 주소</param>
    public void SendPublishButtonData(string topic,string sendData)
    {
        client.Publish("ModelOnOff/" + topic, System.Text.Encoding.Default.GetBytes(sendData));     
    }
 
    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {        
        //TODO test후 꼭 주석처리 해라.
        Debug.Log("M: " + System.Text.Encoding.UTF8.GetString(e.Message));
        string ms = System.Text.Encoding.UTF8.GetString(e.Message);

        // 검증 하고 (보낸 번호와 버튼이 같은지 ) 아니라면 3번 전송.
        if (ms != "Reconnected")
        {
            AllMessageParsing(System.Text.Encoding.UTF8.GetString(e.Message));
            //각 버튼들 정렬 - 현재 받은 값으로 
           isOne = true;
        }
    }

    /// <summary>
    /// 단순한 메시지 변환 1->true   0->false
    /// </summary>
    /// <param name="message">변환할 문자.</param>
    /// <returns></returns>
    public bool GetBoolMessageChange(string message)
    {        
        bool v = false;
        if (message == "1")  
            v = true;
        else if (message == "0")     
            v = false;
        else if (String.IsNullOrEmpty(message))
        {
            v = false;
           // isError = true;
            Debug.Log("empty message:" + v);
        }        
        else if(message == "Reconnected")
        {
            v = false;
           // isError = true;
            Debug.Log("Recon : " + message + ":" + v);
        }
        else
        {
            v = false;
            // isError = true;
            Debug.Log("잘못된 명령 메시지 입니다. : " + message + ":" + v);
        }
        return v;
    }

    void Update()
    {
        if (isOne)
        {
            StartCoroutine(AllButtonSet());            
            isOne = false;          
        }

        //팝업 메시지 띠우기.
       loadingPopUpObject.SetActive(isLoading);
    }

    /// <summary>
    /// 약간의 딜레이를 주기 위해.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AllButtonSet()
    {        
        yield return new WaitForSeconds(.1f);
        AllButtonsSetting();
    }

    /// <summary>
    /// 서버로 부터 받은 정보를 각각의 버튼들에게 전달한다.
    /// </summary>
    public void AllButtonsSetting()
    {
        buttonPowerObject.GetComponent<SwitchingManager>().SetSwitching(GetBoolMessageChange(PowerButtonState));
        button1Object.GetComponent<SwitchingManager>().SetSwitching(GetBoolMessageChange(Button_1_State));
        button2Object.GetComponent<SwitchingManager>().SetSwitching(GetBoolMessageChange(Button_2_State));
        button3Object.GetComponent<SwitchingManager>().SetSwitching(GetBoolMessageChange(Button_3_State));
        button4Object.GetComponent<SwitchingManager>().SetSwitching(GetBoolMessageChange(Button_4_State));
    }
       
    /// <summary>
    /// 서버로 부터 받은 정보를 각 변수에 저장한다.
    /// </summary>
    /// <param name="getMessage">서버로 부터 받은 정보.</param>
    private void AllMessageParsing(string getMessage)
    {
        Button_1_State = GetParserString(getMessage, "|button1=", "|");
        Button_2_State = GetParserString(getMessage, "button2=", "|");
        Button_3_State = GetParserString(getMessage, "button3=", "|");
        Button_4_State = GetParserString(getMessage, "button4=", "|");
        PowerButtonState = GetParserString(getMessage, "buttonPower=", "|");
    }

    /// <summary>
    /// 보낸 버튼의 신호를 확인하고 다른 값이 라면  보낸 값될때 까지 3초간 지속적으로 보낸다.
    /// </summary>
    /// <returns></returns>
    public IEnumerator ReSendToServer()
    {
        SendPublishButtonData(currentButton, currentButtonState);

        yield return new WaitForSeconds(.1f);
        if (currentButton == "button1")
        {
            if (currentButtonState != Button_1_State)
            {
                //Debug.Log("Message ReSend To Server 1");
                isLoading = true;
                SendPublishButtonData(currentButton, currentButtonState);

                if (time < 3)
                    time += .1f;
                else
                {
                    isLoading = false;
                    time = 0;
                    yield return new WaitForSeconds(.1f);
                    yield break;
                }
                //Debug.Log("Message ReSend To Server 2");
              StartCoroutine(ReSendToServer());
            }
            else // 두가지 조건이 같다면   기능 정지.
            {
                isLoading = false;
                time = 0;
                yield break;
            }
        }
        else if (currentButton == "button2")
        {
            if (currentButtonState != Button_2_State)
            {
               // Debug.Log("Message ReSend To Server 1");
                isLoading = true;
                SendPublishButtonData(currentButton, currentButtonState);

                if (time < 3)
                    time += .1f;
                else
                {
                    isLoading = false;
                    time = 0;
                    yield return new WaitForSeconds(.1f);
                    yield break;
                }
                //Debug.Log("Message ReSend To Server 2");
                    StartCoroutine(ReSendToServer());
            }
            else // 두가지 조건이 같다면   기능 정지.
            {
                isLoading = false;
                time = 0;
                yield break;
            }
        }
        else if (currentButton == "button3")
        {
            if (currentButtonState != Button_3_State)
            {
               // Debug.Log("Message ReSend To Server 1");
                isLoading = true;
                SendPublishButtonData(currentButton, currentButtonState);

                if (time < 3)
                    time += .1f;
                else
                {
                    isLoading = false;
                    time = 0;
                    yield return new WaitForSeconds(.1f);
                    yield break;
                }
               // Debug.Log("Message ReSend To Server 2");
                
               StartCoroutine(ReSendToServer());
               
            }
            else // 두가지 조건이 같다면   기능 정지.
            {
                isLoading = false;
                time = 0;
                yield break;
            }
        }
        else if (currentButton == "button4")
        {
            if (currentButtonState != Button_4_State)
            {
                //Debug.Log("Message ReSend To Server 1");
                isLoading = true;
                SendPublishButtonData(currentButton, currentButtonState);

                if (time < 3)
                    time += .1f;
                else
                {
                    isLoading = false;
                    time = 0;
                    yield return new WaitForSeconds(.1f);
                    yield break;
                }

                //Debug.Log("Message ReSend To Server 2");
                
                 StartCoroutine(ReSendToServer());
            }
            else // 두가지 조건이 같다면   기능 정지.
            {
                isLoading = false;
                time = 0;
                yield break;
            }
        }
        else if (currentButton == "buttonPower")
        {
            if (currentButtonState != PowerButtonState)
            {
                //Debug.Log("Message ReSend To Server 1");
                isLoading = true;
                SendPublishButtonData(currentButton, currentButtonState);

                if (time < 3)
                    time += .1f;
                else
                {
                    isLoading = false;
                    time = 0;
                    yield return new WaitForSeconds(.1f);
                    yield break;
                }
               // Debug.Log("Message ReSend To Server 2");
                
               StartCoroutine(ReSendToServer());
               
            }
            else // 두가지 조건이 같다면   기능 정지.
            {
                isLoading = false;
                time = 0;
                yield break;
            }
        }
        else
        {
            Debug.Log("아런 경우도 있나? 버튼이");
        }
    }

 

    /// <summary>
    /// 서버로 부터 받은 정보를 나눈다.
    /// </summary>
    /// <param name="message">서버 data</param>
    /// <param name="startSearch">시작문구</param>
    /// <param name="endSearch">끝 문구</param>
    /// <returns></returns>
    public string GetParserString(string message ,string startSearch,string endSearch)
    {
        string getValue = "";
        string search = "";

        search = startSearch;        
    
        int p = message.IndexOf(search);
        if (p >= 0)
        {
            // move forward to the value
            int start = p + search.Length;
            // now find the end by searching for the next closing tag starting at the start position, 
            // limiting the forward search to the max value length
            int end = 0;
            end = message.IndexOf(endSearch, start);           

            if (end >= 0)
            {
                // pull out the substring
                string v = message.Substring(start, end - start);
                // finally parse into a float
                // float value = float.Parse(v);
                // Debug.Log("1classTemp Value = " + value);
              
               getValue = v;                
            }
            else
            {
                Debug.Log("Bad html - closing tag not found");
                getValue = "text error";
            }
        }
        return getValue;
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
    public string SendOrder(string order)
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
