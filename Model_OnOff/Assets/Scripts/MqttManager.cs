using UnityEngine;
using System.Collections;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System;

public class MqttManager : MonoBehaviour
{
    private MqttClient client;
    /// <summary>
    /// 서버로 부터 받아 파싱후 저장.
    /// </summary>
    public string GetPowerButton;
    public string GetButton_1;
    public string GetButton_2;
    public string GetButton_3;
    public string GetButton_4;
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

    public GameObject errorPopUpObject;
    public GameObject reConnectPopUpObject;
    public bool isError; // error message 들어오면 팝업 띠워주자.
    public bool isReConnect; // 아두이노 wifi통신이 다시 접속했다는 메시지 창.
    public string currentButton;  // 명령 버튼명을 저장후  서버로 부터 받은 번호와 같은지 비교하기 위해 저장.
    public string currentButtonState;// 명령 버튼의 상태를 저장후  서버로 부터 받은 번호와 같은지 비교하기 위해 저장.
    private bool isLoading; // 버튼 누르고 로딩 화면 보여 줄려고.

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
        //TODO test후 꼭 주석처리 해라.
        Debug.Log("M: " + System.Text.Encoding.UTF8.GetString(e.Message));

        //moter constroler의 wifi가 불안정하여 다시 접속했다.
        if (System.Text.Encoding.UTF8.GetString(e.Message) == "Reconnected")
            isReConnect = true;

        // 검증 하고 (보낸 번호와 버튼이 같은지 ) 아니라면 3번 전송.
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
    private bool AllMessageParsing(string getMessage)
    {
        Button_1_State = GetParserString(getMessage, "|button1=", "|");
        Button_2_State = GetParserString(getMessage, "button2=", "|");
        Button_3_State = GetParserString(getMessage, "button3=", "|");
        Button_4_State = GetParserString(getMessage, "button4=", "|");
        PowerButtonState = GetParserString(getMessage, "buttonPower=", "|");
        /*

        //정상.  버튼의 상태를 보여주면 된다.  아니라면  다시 보낸다.
        if (currentButton == "button1" )
        {
            if (currentButtonState != Button_1_State)
            {
                Debug.Log("Button_1_State :   error");
                StartCoroutine(ReSendToServer());
                return false;
            }
    
            다시 명령을 보내라
                다시 신호를 보내라
                    코루틴으로 보내라
                        3번 버내라
                            그중 걸리면 정지 하고 화면을 보여줘라
                                아니면 다시 시도 하세요 문구를 보여줘라 - & 로딩을 보여줘라.
            
        }
        if (currentButton == "button2")
        {
            if (currentButtonState != Button_2_State)
            {
                Debug.Log("Button_2_State :   error");
                return false;
            }
        }
        if (currentButton == "button3")
        {
            if (currentButtonState != Button_3_State)
            {
                Debug.Log("Button_3_State :   error");
                return false;
            }
        }
        if (currentButton == "button4")
        {
            if (currentButtonState != Button_4_State)
            {
                Debug.Log(" Button_4_State error");
                return false;
            }
        }
        if (currentButton == "buttonPower")
        {
            if (currentButtonState != PowerButtonState)
            {
                Debug.Log(" buttonState error");
                return false;
            }
        }
        else
        {
            Debug.Log(" button을 알수 없는 error  &  button : " + currentButton);
            return false;
        }
*/
        return false;
    }

    public IEnumerator ReSendToServer()
    {
        Debug.Log("Re Message Send To Server");
        yield return new WaitForSeconds(.2f);

        if (currentButtonState != Button_1_State)
        {
            isLoading = true;
            yield return new WaitForSeconds(.2f);

            Debug.Log("Button_1_State :   error");
            StartCoroutine(ReSendToServer());
           
        }
        else
        {
            isLoading = false;
            yield break;
        }
        /*
        다시 명령을 보내라
            다시 신호를 보내라
                코루틴으로 보내라
                    3번 버내라
                        그중 걸리면 정지 하고 화면을 보여줘라
                            아니면 다시 시도 하세요 문구를 보여줘라 - & 로딩을 보여줘라.
         * */

        if (currentButton == "button2")
        {
            if (currentButtonState != Button_2_State)
            {
                Debug.Log("Button_2_State :   error");
               
            }
        }
        if (currentButton == "button3")
        {
            if (currentButtonState != Button_3_State)
            {
                Debug.Log("Button_3_State :   error");
                
            }
        }
        if (currentButton == "button4")
        {
            if (currentButtonState != Button_4_State)
            {
                Debug.Log(" Button_4_State error");
               
            }
        }
        if (currentButton == "buttonPower")
        {
            if (currentButtonState != PowerButtonState)
            {
                Debug.Log(" buttonState error");
                
            }
        }
        else
        {
            Debug.Log(" button을 알수 없는 error  &  button : " + currentButton);
            
        }
    }

    /// <summary>
    /// 현제 order 버튼 서버로 부터 받은 버튼 비교
    /// 현제 order 버튼 상태와 서버로 부터 받은 버튼 비교
    /// </summary>
    /// <returns></returns>
    private bool ChackReturnOrder()
    {
        if(GetButton_1 == Button_1_State || currentButtonState == Button_1_State){}
        return false;
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
