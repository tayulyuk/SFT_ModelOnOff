using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전원을 켰을때 나머지 버튼들이 활성화. (이번작업에선 사용하지 않는다)
/// </summary>
public class PowerButtonManager : MonoBehaviour {

    public GameObject moterButtonObject_1;
    public GameObject moterButtonObject_2;
    public GameObject moterButtonObject_3;
    public GameObject moterButtonObject_4;

    void Start()
    {
        MoterButtonState(false);
    }

    public void MoterButtonState(bool state)
    {
        moterButtonObject_1.SetActive(state);
        moterButtonObject_2.SetActive(state);
        moterButtonObject_3.SetActive(state);
        moterButtonObject_4.SetActive(state);
    }
}
