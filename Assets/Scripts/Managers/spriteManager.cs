﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Xml; 

public class spriteSheet
{
	public Dictionary<string, Sprite> sprites;

	public spriteSheet()
	{
		sprites = new Dictionary<string, Sprite>();
	}
}
public class spriteManager : MonoBehaviour // sprite magaer loads sprite(sheets) from the streamingassets folder
{
	static public spriteManager instance;

	Dictionary<string, spriteSheet> spriteSheets;

	// Use this for initialization
	void OnEnable () 
	{
		instance = this;

		loadSprites();
	}
	void loadSprites()
	{
		instance.spriteSheets = new Dictionary<string, spriteSheet>();

		string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "sprites");
		loadInDirs(filePath);
	}
	void loadInDirs(string filePath) // recursively loops all dirs and loads their spritesheet
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
			if(fileNames.Length == 0)
			{
				// No files in the directory, just return
				Debug.Log("spriteManager::getDirs: No files in " + filePath);
				return;
			}
			else
			{
				for (int i = 0; i < fileNames.Length; i++)
				{
					if (fileNames[i].Contains(".png")) // we only want to load .png files
													   // FIXME this will load any file that contains .png, not just ones that have it at the end
					{
						if (!fileNames[i].Contains(".meta")) // only call if its NOT a meta file
						{
							this.spriteSheets.Add(System.IO.Path.GetFileNameWithoutExtension(fileNames[i]), loadSpriteSheet(fileNames[i]));
						}
					}
				}
			}
		}
	}
	spriteSheet loadSpriteSheet(string filePath) // loads a sprite sheet and puts it in our dictionary
	{
		spriteSheet sheet = new spriteSheet();
		Texture2D imgText = new Texture2D(2, 2); // load a placeholder texture

		// now we load in the spritesheet and its XML file, then we load all the sprites based on it
		byte[] bytes = System.IO.File.ReadAllBytes(filePath);
		imgText.LoadImage(bytes);

		// now we find the XML file, and we load it
		string baseSpritePath = System.IO.Path.GetFileNameWithoutExtension(filePath);
		string xmlPath = System.IO.Path.Combine(System.IO.Directory.GetParent(filePath).FullName, baseSpritePath + ".xml"); // NOTE the xml extension must be in LOWER CASE 

		try
		{
			string tmp = System.IO.File.ReadAllText(xmlPath);
		}
		catch
		{
			// there was no xml file for the sheet
			Debug.Log("spriteManager::loadSpriteSheet: No XML file for " + filePath);
			return null;
		}
		XmlTextReader reader = new XmlTextReader(xmlPath);

		reader.ReadToDescendant("sprites");
		if (!reader.ReadToDescendant("sprite"))
		{
			// we have an empty file!
			Debug.LogError("spriteManager::loadSpriteSheet: No <sprite> in " + filePath + ".xml");
			return null;
		}
		else
		{
			string name;
			Sprite sprite = loadSpriteFromXML(imgText, reader, out name);
			sheet.sprites.Add(name, sprite);
		}
		while (reader.ReadToNextSibling("sprite"))
		{
			string name;
			Sprite sprite = loadSpriteFromXML(imgText, reader, out name);
			sheet.sprites.Add(name, sprite);
		}
		return sheet;

	}
	Sprite loadSpriteFromXML(Texture2D imgText, XmlTextReader reader, out string outName) 
		// NOTE this renders incorrect edges VERY rarely (ie. when moving AND scaling the camera), 
		// if this becomes a problem in the future, create a 2 pixel border around every sprite, of the same edge colour, 
		// this will eliminate the issue, but since it is VERY rare occurence, 
		// i believe it will be fine. It is not very noticeable at all.
		// VERIFY that both AA and AO are off in ALL quality settings, this will keep the errors few and far between
	{
		string name = reader.GetAttribute("name");
		int x = int.Parse(reader.GetAttribute("x")); // read the attributes, set the values
		int y = int.Parse(reader.GetAttribute("y"));
		int w = 0; // placeholder values, will get set in try-catch block
		int h = 0;
		try
		{
			w = int.Parse(reader.GetAttribute("w"));
		}
		catch
		{
			w = 64;
		}
		try
		{
			h = int.Parse(reader.GetAttribute("h"));
		}
		catch
		{
			h = 64;
		}
		outName = name;
		Sprite sprite = Sprite.Create(imgText, new Rect(x, y, w, h), new Vector2(0.0f, 0.0f), 64); // TODO remove the hardcoded pivot point and PPU
		sprite.texture.filterMode = FilterMode.Point;
		return sprite;
	}
	
	public Sprite getSprite(string sheetName, string spriteName) // gets s sprite from the list by name
	{
		if (instance.spriteSheets.ContainsKey(sheetName))
		{
			if (instance.spriteSheets[sheetName].sprites.ContainsKey(spriteName))
			{
				return instance.spriteSheets[sheetName].sprites[spriteName];
			}
		}
		Debug.Log("spriteManager::getSprite: No sprite of name " + spriteName + " in " + sheetName);
		return null; // TODO maybe return a purple square
	}
}
