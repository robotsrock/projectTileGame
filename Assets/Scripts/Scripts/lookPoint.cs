using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class lookPoint : MonoBehaviour 
{
	[Header("Objects")]
	public GameObject characterController;
	// Use this for initialization
	void Start () 
	{
		characterController = GameObject.FindGameObjectWithTag("characterController");
		if (characterController == null)
		{
			Debug.LogError("No character controller found");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	void OnTriggerEnter()
	{
		characterController.GetComponent<characterController>().isBlocked = true;
	}
	void OnTriggerExit()
	{
		characterController.GetComponent<characterController>().isBlocked = false;
	}
}
