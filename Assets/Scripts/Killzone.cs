using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;


public class Killzone : MonoBehaviour
{


	void OnTriggerEnter2D(Collider2D other)
	{
//		Debug.Log("Collision:" + other.name);
		other.gameObject.SetActive(false);
	}
}