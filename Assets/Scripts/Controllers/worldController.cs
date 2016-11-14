using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class tileSpriteSet // each has a set of sprites for that tile, 0 is an empty tile so it has no sprites
{
	public Sprite[] spriteSet;
}


public class worldController : MonoBehaviour // REFACTOR we need to use a list for GOs
{
	[Header("Variables")]
	public int worldWidth = 10;
	public int worldHeight = 10;

	public world firstWorld { get; protected set; } // world contains the array of tiles

	tileSpriteController tileController;
	characterController charController;
	WOspriteController WOcontroller;

	// Use this for initialization
	void Start ()
	{
		if (firstWorld == null)
		{
			this.setupWorld();
		}
	}
	
	public void setupWorld()
	{
		firstWorld = new world();
		firstWorld.setupWorld(this.worldWidth, this.worldHeight);

		tileController = GameObject.FindGameObjectWithTag("tileSpriteController").GetComponent<tileSpriteController>();
		charController = GameObject.FindGameObjectWithTag("characterController").GetComponent<characterController>();
		WOcontroller   = GameObject.FindGameObjectWithTag("WOspriteController").GetComponent<WOspriteController>();

		generateWorld();

		Debug.Log("World is setup");
	}
	void generateWorld()
	{
		for (int x = 0; x < this.worldWidth; x++)
		{
			for (int y = 0; y < this.worldHeight; y++)
			{
				tileController.addGO(x, y, firstWorld.getTileAt(x, y));
			}
		}
		charController.addGO(firstWorld.mainCharacter);
	}
}
