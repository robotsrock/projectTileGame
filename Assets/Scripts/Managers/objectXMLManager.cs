using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Xml;

public class objectXMLManager : MonoBehaviour 
{
	protected class xmlIndex
	{
		public string author;
	}

	static public objectXMLManager instance;

	public Dictionary<string, worldObject> worldObjectProtos { get; protected set; }

	void Awake()
	{
		instance = this;

		loadPrototypes();
	}
	void loadPrototypes()
	{
		worldObjectProtos = new Dictionary<string, worldObject>();

		string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "data");
		loadInDirs(filePath);
	}
	void loadInDirs(string filePath)
	{
		string[] subDirectories = System.IO.Directory.GetDirectories(filePath);
		if (subDirectories.Length > 0)
		{
			for (int i = 0; i < subDirectories.Length; i++)
			{
				loadInDirs(subDirectories[i]);
			}
		}
		else
		{
			string[] fileNames = System.IO.Directory.GetFiles(filePath);
			if (fileNames.Length == 0)
			{
				// No files in the directory, just return
				Debug.Log("objectXMLManager::loadInDirs: No files in " + filePath);// FIXME meta files are evil
				return;
			}
			else
			{
				for (int i = 0; i < fileNames.Length; i++)
				{
					if (fileNames[i].Contains(".xml")) // we only want to load .xml files
													   // NOTE this will load any file that contains .xml, not just ones that have it at the end
					{
						if (!fileNames[i].Contains(".meta")) // only call if its NOT a meta file
						{
							xmlIndex index = getIndexData(fileNames[i]); // TODO use the index data for something useful
							loadXML(fileNames[i]);
						}
					}
				}
			}
		}
	}
	xmlIndex getIndexData(string filePath) // gets date from <index>
	{
		XmlTextReader reader = new XmlTextReader(filePath);
		if (!reader.ReadToDescendant("index"))
		{
			Debug.LogError("spriteManager::getIndexData: No <index> in " + filePath);
			return null;
		}
		string author = null;
		author = xmlHelperManager.getStringRequired("author", reader);

		xmlIndex tmpIndex = new xmlIndex();
		tmpIndex.author = author;
		return tmpIndex;
	}
	void loadXML(string filePath)
		// NOTE we assume that the name for the sprite sheet is the SAME as the name of the object
	{
		XmlTextReader reader = new XmlTextReader(filePath);
		reader.ReadToDescendant("prototypes");
		if (!reader.ReadToDescendant("prototype"))
		{
			// we have an empty file!
			Debug.LogError("objectXMLManager::loadProto: No <prototype> in " + filePath);
			return;
		}
		else
		{
			loadProto(reader);
        }
		while(reader.ReadToNextSibling("prototype"))
		{
			loadProto(reader);
        }		// TODO load other XML stuff in this function
		return;
	}
	worldObject loadProtoFromXML(XmlTextReader reader, worldObject parentObj)
	{
		string type = null;
		string baseType = null;
		float moveRate = 1.0f;
		int width = 1;// placeholder
		int height = 1;

		if (parentObj == null)
		{
			type = xmlHelperManager.getStringRequired("type", reader);
			baseType = xmlHelperManager.getStringRequired("baseType", reader);
			moveRate = xmlHelperManager.getFloatRequired("moveRate", reader);
			width = xmlHelperManager.getIntDefault("w", 1, reader);
			height = xmlHelperManager.getIntDefault("h", 1, reader);
		}
		else
		{
			type = xmlHelperManager.getStringRequired("type", reader);
			baseType = parentObj.baseType;
			moveRate = xmlHelperManager.getFloatDefault("moveRate", parentObj.movementCost, reader);
			width = xmlHelperManager.getIntDefault("w", parentObj.width, reader);
			height = xmlHelperManager.getIntDefault("h", parentObj.height, reader);
		}

		worldObject obj = worldObject.createPrototype(type, moveRate, width, height, baseType);
		return obj;
	}
	void loadProto(XmlTextReader reader)
	{
		worldObject parentProto = loadProtoFromXML(reader, null);         // loads the first proto encountered
		worldObjectProtos.Add(parentProto.objectType, parentProto);
		if (reader.ReadToDescendant("variant"))                         // loads its variants
		{
			worldObject childProto = loadProtoFromXML(reader, parentProto);
			worldObjectProtos.Add(childProto.objectType, childProto);
			
			while (reader.ReadToNextSibling("variant"))
			{
				childProto = loadProtoFromXML(reader, parentProto);
				worldObjectProtos.Add(childProto.objectType, childProto);
			}
		}
	}
}
