using UnityEngine;
using System.Collections;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;

using System;

public class MqttTest : MonoBehaviour {
	private MqttClient client;
    private bool isUiRoot;
	// Use this for initialization

    public UILabel tempLabel;
    public UILabel humiLabel;

    public string mes;

    public string GetMessage()
    {
        return mes;
    }
	void Start () {
		// create client instance 
        client = new MqttClient(IPAddress.Parse("119.205.235.214"), 1883, false, null); 
		
		// register to message received 
		client.MqttMsgPublishReceived += client_MqttMsgPublishReceived; 
		
		string clientId = Guid.NewGuid().ToString(); 
		client.Connect(clientId); 
		
		// subscribe to the topic "/home/temperature" with QoS 2 
		client.Subscribe(new string[] { "ggg" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                   
       // StartCoroutine(OneSecondGetValue(currentStateMessage));       
	}

    private IEnumerator OneSecondGetValue(string message)
    {
        Debug.Log(message);
        //tempLabel.text = GetParserString(message, "temp");
       // humiLabel.text = GetParserString(message, "humi");
       
        yield return new WaitForSeconds(1);
        StartCoroutine(OneSecondGetValue(GetMessage()));      
    }
	void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
	{
           mes = System.Text.Encoding.UTF8.GetString(e.Message);
		Debug.Log("Received: " + System.Text.Encoding.UTF8.GetString(e.Message)  );
	}

    private string GetParserString(string message,string order)
    {
        string getValue = "";
        string search = "";
           
        if (order == "temp")
        {
            search = "\"temperature\":";
        }
        if (order == "humi")
        {
            search = "\"humidity\":";
        }
        int p = message.IndexOf(search);
        if (p >= 0)
        {
            // move forward to the value
            int start = p + search.Length;
            // now find the end by searching for the next closing tag starting at the start position, 
            // limiting the forward search to the max value length
            int end = 0;
            if(order == "temp")
                 end = message.IndexOf(",", start);
            if(order == "humi")
                end = message.IndexOf("}", start);

            if (end >= 0)
            {
                // pull out the substring
                string v = message.Substring(start, end - start);
                // finally parse into a float
                // float value = float.Parse(v);
                // Debug.Log("1classTemp Value = " + value);
                if (order == "temp")
                    getValue = v + " ℃";
                if(order == "humi")
                    getValue = v + " ％";
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
