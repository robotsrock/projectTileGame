using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Xml;

public class xmlHelperManager : MonoBehaviour 
{
	public static int getIntRequired(string attributeName, XmlTextReader reader) // returns an int from a reader that is required
	{
		int tmp;
		if (!int.TryParse(reader.GetAttribute(attributeName), out tmp))
		{
			Debug.LogError("xmlHelperManager::getIntRequired: required int attribute error");
			return 0;
		}
		return tmp;
	}
	public static int getIntDefault(string attributeName, int def, XmlTextReader reader) // returns an int, but uses the default if it cant parse
	{
		int tmp;
		try
		{
			tmp = int.Parse(reader.GetAttribute(attributeName));
		}
		catch
		{
			tmp = def;
		}
		return tmp;
	}
	public static float getFloatRequired(string attributeName, XmlTextReader reader) // returns an float from a reader that is required
	{
		float tmp;
		if (!float.TryParse(reader.GetAttribute(attributeName), out tmp))
		{
			Debug.LogError("xmlHelperManager::getFloatRequired: required float attribute error");
			return 0.0f;
		}
		return tmp;
	}
	public static float getFloatDefault(string attributeName, float def, XmlTextReader reader) // returns an float, but uses the default if it cant parse
	{
		float tmp;
		try
		{
			tmp = int.Parse(reader.GetAttribute(attributeName));
		}
		catch
		{
			tmp = def;
		}
		return tmp;
	}
	public static string getStringRequired(string attributeName, XmlTextReader reader) // returns an int from a reader that is required
	{
		string tmp;
		tmp = reader.GetAttribute(attributeName);
		if (tmp == null)
		{
			Debug.LogError("xmlHelperManager::getIntRequired: required string attribute error");
			return null;
		}
		return tmp;
	}
	public static string getStringDefault(string attributeName, string def, XmlTextReader reader) // returns an int, but uses the default if it cant parse
	{
		string tmp;
		try
		{
			tmp = reader.GetAttribute(attributeName);
		}
		catch
		{
			tmp = def;
		}
		return tmp;
	}
}
