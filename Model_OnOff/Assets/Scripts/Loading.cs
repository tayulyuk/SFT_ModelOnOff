using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 로딩 ...
/// </summary>
public class Loading : MonoBehaviour
{
    public int speed = 1000;
	void Update () {
		transform.Rotate(0,0,-Time.deltaTime * speed * 100);
	}
}
