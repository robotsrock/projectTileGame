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
	[Header("Objects")]
	public GameObject lookPointPrefab;

	world firstWorld; // world contains the array of tiles
	GameObject characterGO;
	character mainChar;
	Sprite wallSprite;

	Dictionary<worldObject, GameObject> worldObjects;

	tileSpriteController tileController;

	void Awake()// gets run right at the start
	{

	}
	// Use this for initialization
	void Start ()
	{
		this.setupWorld();
		this.generateWorld();
	}
	
	// Update is called once per frame
	void Update ()
	{

	}
	void setupWorld()
	{
		firstWorld = new world();
		firstWorld.setupWorld(this.worldWidth, this.worldHeight);
		this.mainChar = firstWorld.getMainCharacter();
		worldObjects = new Dictionary<worldObject, GameObject>();

		firstWorld.registerWorldObjectCreatedCB(onWorldObjectCreated);
		firstWorld.registerWorldObjectDestroyedCB(onWorldObjectDestroyed);
		this.wallSprite = spriteManager.instance.getSprite("wall-sheet", "wall_b_0"); //! NOTE TMP code, this is a placeholder for the first OBJ

		tileController = GameObject.FindGameObjectWithTag("tileSpriteController").GetComponent<tileSpriteController>();
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
		GameObject charGO = new GameObject("mainCharacter_" + this.mainChar.getName());
		charGO.transform.SetParent(this.transform);
		charGO.transform.position = new Vector3(mainChar.getPosition().x, mainChar.getPosition().y, -2); // setup the main character game object
		charGO.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
		SpriteRenderer charSR = charGO.AddComponent<SpriteRenderer>();
		charSR.sprite = this.characterSprite;

		GameObject lpGO = (GameObject)Instantiate(this.lookPointPrefab, charGO.transform);
		lpGO.transform.localPosition = new Vector3(0.1f, 0, 2);
		lpGO.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);

		this.characterGO = charGO;
		this.mainChar.registerMoveCallback((character) => { this.onCharMoved(character, charGO); });
	}

	public void onCharMoved(character charData, GameObject charGO) // when a character moves, we update the GO pos
	{
		charGO.transform.position = new Vector3(charData.getPosition().x, charData.getPosition().y, -2);
	}
	public world getWorld()
	{
		return this.firstWorld;
	}
	public character getMainChar()
	{
		return this.mainChar;
	}
	public GameObject getMainCharGO()
	{
		return this.characterGO;
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
