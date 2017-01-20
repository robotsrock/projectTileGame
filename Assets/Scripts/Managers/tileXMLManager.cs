using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Xml;

public class tileXMLManager : MonoBehaviour 
{
	protected class xmlIndex
	{
		public string author;
	}
	static public tileXMLManager instance;

	public Dictionary<string, tile> tileProtos { get; protected set; }

	void Awake()
	{
		instance = this;

		loadPrototypes();
	}
	void loadPrototypes()
	{
		tileProtos = new Dictionary<string, tile>();

		string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "data");
		filePath = System.IO.Path.Combine(filePath, "tiles");
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
	xmlIndex getIndexData(string filePath) // gets data from <index>
	{
		XmlTextReader reader = new XmlTextReader(filePath);
		if (!reader.ReadToDescendant("index"))
		{
			Debug.LogError("tileXMLManager::getIndexData: No <index> in " + filePath);
			return null;
		}
		string author = null;
		author = xmlHelperManager.getStringRequired("author", reader);

		xmlIndex tmpIndex = new xmlIndex();
		tmpIndex.author = author;
		return tmpIndex;
	}
	void loadXML(string filePath)
	// NOTE name of the spritesheet is <name>
	{
		XmlTextReader reader = new XmlTextReader(filePath);
		reader.ReadToDescendant("prototypes");
		if (!reader.ReadToDescendant("prototype"))
		{
			// we have an empty file!
			Debug.LogError("tileXMLManager::loadProto: No <prototype> in " + filePath);
			return;
		}
		else
		{
			loadProto(reader);
		}
		while (reader.ReadToNextSibling("prototype"))
		{
			loadProto(reader);
		}       // TODO load other XML stuff in this function
		return;
	}
	void loadProto(XmlTextReader reader)
	{
		tile parentProto = loadProtoFromXML(reader, null);         // loads the first proto encountered
		tileProtos.Add(parentProto.tileType, parentProto);
		if (reader.ReadToDescendant("variant"))                         // loads its variants
		{
			tile childProto = loadProtoFromXML(reader, parentProto);
			tileProtos.Add(childProto.tileType, childProto);

			while (reader.ReadToNextSibling("variant"))
			{
				childProto = loadProtoFromXML(reader, parentProto);
				tileProtos.Add(childProto.tileType, childProto);
			}
		}
	}
	tile loadProtoFromXML(XmlTextReader reader, tile parentObj)
	{
		string type = null;
		string baseType = null;
		//float moveRate = 1.0f;

		if (parentObj == null) // TODO implement moveRate
		{
			type = xmlHelperManager.getStringRequired("type", reader);
			baseType = xmlHelperManager.getStringRequired("baseType", reader);
			//moveRate = xmlHelperManager.getFloatRequired("moveRate", reader);
		}
		else
		{
			type = xmlHelperManager.getStringRequired("type", reader);
			baseType = parentObj.baseType;
			//moveRate = xmlHelperManager.getFloatDefault("moveRate", parentObj.movementCost, reader);
		}

		tile t = tile.createPrototype(type, baseType);
		return t;
	}
}
