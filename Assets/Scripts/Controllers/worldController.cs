using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class tileSpriteSet // each has a set of sprites for that tile, 0 is an empty tile so it has no sprites
{
	public Sprite[] spriteSet;
}


public class worldController : MonoBehaviour // REFACTOR use properties, also we need to use a list for GOs
	// REFACTOR split this into indovidual sprite controllers
{
	[Header("Variables")]
	public int worldWidth = 10;
	public int worldHeight = 10;
	[Header("Sprites")]
	[Space(5)]
	public tileSpriteSet[] allTileSprites; // FIXME move this to a dedicated tile sprite file, this code is not easy to read
										   // TODO load from a file
	public Sprite characterSprite;

	world firstWorld; // world contains the array of tiles
	Sprite wallSprite;

	Dictionary<worldObject, GameObject> worldObjects;

	tileSpriteController tileController;
	characterController charController;

	// Use this for initialization
	void Start ()
	{
		this.setupWorld();
		this.generateWorld();
	}
	
	void setupWorld()
	{
		firstWorld = new world();
		firstWorld.setupWorld(this.worldWidth, this.worldHeight);
		worldObjects = new Dictionary<worldObject, GameObject>();

		firstWorld.registerWorldObjectCreatedCB(onWorldObjectCreated);
		firstWorld.registerWorldObjectDestroyedCB(onWorldObjectDestroyed);
		this.wallSprite = spriteManager.instance.getSprite("wall-sheet", "wall_b_0"); //! NOTE TMP code, this is a placeholder for the first OBJ

		tileController = GameObject.FindGameObjectWithTag("tileSpriteController").GetComponent<tileSpriteController>();
		charController = GameObject.FindGameObjectWithTag("characterController").GetComponent<characterController>();
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
	public world getWorld()
	{
		return this.firstWorld;
	}
	public void onWorldObjectCreated(worldObject obj)
	{
		// create a GO for the obj
		GameObject objGO = new GameObject();

		worldObjects.Add(obj, objGO); // add it to the list

		objGO.name = obj.objectType + "_" + obj.baseTile.position.x + "_" + obj.baseTile.position.y;
        objGO.transform.SetParent(this.transform);
		objGO.transform.position = new Vector3(obj.baseTile.position.x, obj.baseTile.position.y, 0);
		SpriteRenderer tileSR = objGO.AddComponent<SpriteRenderer>();
		tileSR.sprite = this.wallSprite;
		objGO.AddComponent<BoxCollider>();

		obj.registerOnChangedCB(onWorldObjectChanged);
	}
	public void onWorldObjectDestroyed(worldObject obj)
	{
		if (obj != null)
		{
			if (this.worldObjects.ContainsKey(obj))
			{
				this.worldObjects[obj].transform.SetParent(null);
				Destroy(this.worldObjects[obj]);
			}
		}
	}
	public void onWorldObjectChanged(worldObject obj)
	{
		worldObjects[obj].GetComponent<SpriteRenderer>().sprite = spriteManager.instance.getSprite(
			"wall-sheet",
			"wall_b_" + obj.flags); //! NOTE TMP code, we need dedicated sprite controllers DESPERATELY
	}
}
