using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class tileSpriteSet // each has a set of sprites for that tile, 0 is an empty tile so it has no sprites
{
	public Sprite[] spriteSet;
}


public class worldController : MonoBehaviour
{
	[Header("Variables")]
	public int worldWidth = 10;
	public int worldHeight = 10;
	[Header("Sprites")]
	[Space(5)]
	public tileSpriteSet[] allTileSprites; // FIXME move this to a dedicated tile sprite file, this code is not easy to read
										   // TODO load from a file
	public Sprite[] allObjectSprites;	   // TODO load spritesheets from XML
	public Sprite characterSprite;

	world firstWorld; // world contains the array of tiles
	GameObject[,] tileGOs;
	GameObject characterGO;
	character mainChar;
	Vector2 lastMoveDir;

	void Awake()// gets run right at the start
	{
		this.setupWorld(); // HACK we should't do it like this, its very subjective. 
		this.generateWorld();
	}
	// Use this for initialization
	void Start ()
	{
		this.setupAxis(); // HACK Find a better way to load all input bindings after the manager gets setup, maybe a buffer?
    }
	
	// Update is called once per frame
	void Update ()
	{

	}
	void setupWorld()
	{
		tileGOs = new GameObject[this.worldWidth, this.worldHeight];
		firstWorld = new world();
		firstWorld.setupWorld(this.worldWidth, this.worldHeight);
		this.mainChar = firstWorld.getMainCharacter();
	}
	void generateWorld()
	{
		for (int x = 0; x < this.worldWidth; x++)
		{
			for (int y = 0; y < this.worldHeight; y++)
			{
				tile tileAtPos = firstWorld.getTileAt(x, y);

				GameObject tileGO = new GameObject("tile_" + x + "_" + y);
				tileGO.transform.SetParent(this.transform);
				tileGO.transform.position = new Vector3(x, y, 0);
				SpriteRenderer tileSR = tileGO.gameObject.AddComponent<SpriteRenderer>();
				tileSR.sprite = allTileSprites[(int)tileAtPos.getTileType()].spriteSet[tileAtPos.getTileVariant()]; // uses the tile's type and variant to get the right sprite

				tileGOs[x, y] = tileGO;

				tileAtPos.registerSetCallback((tile) => { this.onTileChanged(tile, tileGO); });
			}
		}



		GameObject charGO = new GameObject("mainCharacter_" + this.mainChar.getName());
		charGO.transform.SetParent(this.transform);
		charGO.transform.position = new Vector3(mainChar.getPosition().x, mainChar.getPosition().y, -1); // setup the main character game object
		charGO.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
		SpriteRenderer charSR = charGO.AddComponent<SpriteRenderer>();
		charSR.sprite = this.characterSprite;

		this.characterGO = charGO;
		this.mainChar.registerMoveCallback((character) => { this.onCharMoved(character, charGO); });
	}
	public void onTileChanged(tile tileData, GameObject tileGO) // when a tile changes type or variant, we update the sprite
	{
		tileGO.GetComponent<SpriteRenderer>().sprite = allTileSprites[(int)tileData.getTileType()].spriteSet[tileData.getTileVariant()];
	}
	public void onCharMoved(character charData, GameObject charGO) // when a character moves, we update the GO pos
	{
		
		charGO.transform.position = new Vector3(charData.getPosition().x, charData.getPosition().y, -1);
	}
	public void randomizeTiles()
	{
		for (int x = 0; x < this.worldWidth; x++)
		{
			for (int y = 0; y < worldHeight; y++)
			{
				int rand = Random.Range(1, 3);
				this.firstWorld.getTileAt(x, y).setTile((tileType)rand, 0);
			}
		}
	}
	void setupAxis()
	{
		inputManager.instance.bindAxis("horizontal", "joy1_LSX", KeyCode.D, KeyCode.A);
		inputManager.instance.bindAxis("vertical", "joy1_LSY", KeyCode.W, KeyCode.S);
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
}
