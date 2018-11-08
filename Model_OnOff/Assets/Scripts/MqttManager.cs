using UnityEngine;
using System.Collections;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System;

public class MqttManager : MonoBehaviour
{
    private MqttClient client;    
    public string PowerButtonState;
    public string Button_1_State;
    public string Button_2_State;
    public string Button_3_State;
    public string Button_4_State;
    public bool isOne; // broad cast 사용하기 위해.    

    public GameObject buttonPowerObject;
    public GameObject button1Object;
    public GameObject button2Object;
    public GameObject button3Object;
    public GameObject button4Object;

    public GameObject errorPopUpObject;
    public GameObject reConnectPopUpObject;
    public bool isError; // error message 들어오면 팝업 띠워주자.
    public bool isReConnect; // 아두이노 wifi통신이 다시 접속했다는 메시지 창.

    void Start()
    {
        // create client instance 
        client = new MqttClient(IPAddress.Parse("119.205.235.214"), 1883, false, null);

        // register to message received 
        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

       string clientId = Guid.NewGuid().ToString();
        //string clientId = "siheung_namu_moter";
        client.Connect(clientId);

        // subscribe to the topic "/home/temperature" with QoS 2 
        client.Subscribe(new string[] { "ModelOnOff/result" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
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
        Debug.Log("M: " + System.Text.Encoding.UTF8.GetString(e.Message));

        //moter constroler의 wifi가 불안정하여 다시 접속했다.
        if (System.Text.Encoding.UTF8.GetString(e.Message) == "Reconnected")
            isReConnect = true;

        ///test 끝나고 다시 연결해라.
        AllMessageParsing(System.Text.Encoding.UTF8.GetString(e.Message));    
        //각 버튼들 정렬 - 현재 받은 값으로
        isOne = true;   
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
        else if (message == " " && message == null)
        {
            v = false;
            isError = true;
            Debug.Log("empty message:" + v);
        }        
        else 
        {
            v = false;
            isError = true;
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
       errorPopUpObject.SetActive(isError);
       
        //아두이도 접속 창 띠우기.
       reConnectPopUpObject.SetActive(isReConnect);
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

}
