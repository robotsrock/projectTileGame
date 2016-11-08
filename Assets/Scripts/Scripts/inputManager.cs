using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class axisData // TODO maybe have a list for all positives and one for all negatioves so that we can use multiple buttons?
{
	public string axisName;
	public KeyCode posKeycode = KeyCode.None;
	public KeyCode negKeycode = KeyCode.None;
	public bool isOnGamePad;
}

public class inputManager
{
	Dictionary<string, KeyCode> buttonDictionary; // TODO maybe add in mouse button functionality? also maybe multiple buttons like we have for axis data
	Dictionary<string, axisData> axisDictionary;  // TODO unbind functions
	bool useGamePad = false;
	bool hasBeenSetup = false;

	public static inputManager instance{ get; private set; }
	public inputManager()
	{
		if (instance == null)
		{
			instance = this;
		}
	}
	public void setupManager(bool useGamePad)
	{
		instance.axisDictionary = new Dictionary<string, axisData>();
		instance.buttonDictionary = new Dictionary<string, KeyCode>();
		instance.setUseGamePad(useGamePad);
		instance.hasBeenSetup = true;
	}

	public void bindButton(string buttonName, KeyCode keyCode)
	{
		if (instance.hasBeenSetup)
		{
			instance.buttonDictionary.Add(buttonName, keyCode);
		}
		else
		{
			Debug.LogError("inputManager has not been set up!");
		}
	}
	public void bindAxis(string axisName, string unityAxisName, KeyCode posKey, KeyCode negKey, bool isOnGamePad = true)
	{
		if (instance.hasBeenSetup)
		{
			axisData data = new axisData();
			data.axisName = unityAxisName;
			data.posKeycode = posKey;
			data.negKeycode = negKey;
			data.isOnGamePad = isOnGamePad;
			instance.axisDictionary.Add(axisName, data);
		}
		else
		{
			Debug.LogError("inputManager has not been set up!");
		}
	}

	public bool getButton(string buttonName)
	{
		if (instance.buttonDictionary.ContainsKey(buttonName))
		{
			return Input.GetKey(instance.buttonDictionary[buttonName]);
		}
		else
		{
			Debug.Log("inputManager::getButton: No button of name " + buttonName);
			return false;
		}
	}
	public bool getButtonDown(string buttonName)
	{
		if (instance.buttonDictionary.ContainsKey(buttonName))
		{
			return Input.GetKeyDown(instance.buttonDictionary[buttonName]);
		}
		else
		{
			Debug.Log("inputManager::getButtonDown: No button of name " + buttonName);
			return false;
		}
	}
	public bool getButtonUp(string buttonName)
	{
		if (instance.buttonDictionary.ContainsKey(buttonName))
		{
			return Input.GetKeyUp(instance.buttonDictionary[buttonName]);
		}
		else
		{
			Debug.Log("inputManager::getButtonUp: No button of name " + buttonName);
			return false;
		}
	}
	public float getAxis(string axisName) // TODO prevent binding if it already exists?
	{
		if (instance.axisDictionary.ContainsKey(axisName))
		{
			if (instance.useGamePad || !instance.axisDictionary[axisName].isOnGamePad) // if the manager is set to use gamepad input, or if this axis isnt on GP
			{
                return Input.GetAxis(instance.axisDictionary[axisName].axisName);
			}
			else
			{
				if (Input.GetKey(instance.axisDictionary[axisName].posKeycode))
				{
					return 1f;
				}
				else if (Input.GetKey(instance.axisDictionary[axisName].negKeycode))
				{
					return -1f;
				}
				else
				{
					return 0;
				}
            }
		}
		else
		{
			Debug.Log("inputManager::getAxis: No axis of name " + axisName);
			return 0f;
		}
	}
	public void setUseGamePad(bool value)
	{
		instance.useGamePad = value;
	}
}
