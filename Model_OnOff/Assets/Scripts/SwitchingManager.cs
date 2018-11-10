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
   
}
