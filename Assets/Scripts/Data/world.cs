using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class world // REFACTOR use properties
{
	tile[,] worldTiles;
	character mainCharacter;

	Dictionary<string, worldObject> worldObjectProtos;
	Action<worldObject> onWorldObjectCreatedCB;
	Action<worldObject> onWorldObjectDestroyedCB;

	public world()
	{

	}
	public void setupWorld(int worldWidth, int worldHeight)
	{
		this.worldTiles = new tile[worldWidth, worldHeight];
		this.mainCharacter = new character(new Vector2(5, 5), "Dave", 0.05f, this); // FIXME this should be loaded somewhere else, it should have easy to edit vars
		for (int x = 0; x < worldWidth; x++)
		{
			for (int y = 0; y < worldHeight; y++)
			{
				this.worldTiles[x, y] = new tile(tileType.grass, 0, new Vector2(x, y), this); // create a basic grass tile for all tiles (they wil be changed later)
																						// TODO create a proper world generator, and call it here
			}
		}

		this.setupPrototypes();
	}
	public tile getTileAt(int x, int y)
	{
		return this.worldTiles[x, y];
	}
	public worldObject getWorldObjectAt(int x, int y)
	{
		return this.worldTiles[x, y].childObject;
	}
	public void setTileAt(int x, int y, tileType type, int variant)
	{
		this.worldTiles[x, y].setTile(type, variant);
	}
	public character getMainCharacter()
	{
		return this.mainCharacter;
	}

	public void setupPrototypes()
	{
		this.worldObjectProtos = new Dictionary<string, worldObject>(); // TODO read from XML file
		this.createObjectPrototype("wall", 0.0f, 1, 1);
	}
	public void createObjectPrototype(string type, float movementCost, int width, int height)
	{
		worldObject obj = worldObject.createPrototype(
			type,
			movementCost,
			width,
			height);
		this.worldObjectProtos.Add(type, obj);
	}
	public void placeWorldObject (string objectType, tile t)
	{
		// TODO make it use rotation and big objects
		if (worldObjectProtos.ContainsKey(objectType))
		{
			worldObject obj = worldObject.placeInstance(worldObjectProtos[objectType], t);
			//Debug.Log("Placed object");
			if (obj != null)
			{
				if (onWorldObjectCreatedCB != null)
				{
					onWorldObjectCreatedCB(obj);
				}
			}
		}
		else
		{
			Debug.Log("world::placeWorldOject: No object of type " + objectType);
		}
		this.updateAdjacentTiles(t); // update the adjacent tiles
	}
	public void destroyWorldObject(tile t)
	{
		if (onWorldObjectDestroyedCB != null)
		{
			onWorldObjectDestroyedCB(t.childObject);
		}
		worldObject.destroyInstance(t);
		this.updateAdjacentTiles(t); // update the adjacent tiles
	}
	public void registerWorldObjectCreatedCB(Action<worldObject> callback)
	{
		onWorldObjectCreatedCB += callback;
	}
	public void unRegisterWorldObjectCreatedCB(Action<worldObject> callback)
	{
		onWorldObjectCreatedCB -= callback;
	}
	public void registerWorldObjectDestroyedCB(Action<worldObject> callback)
	{
		onWorldObjectDestroyedCB += callback;
	}
	public void unRegisterWorldObjectDestroyedCB(Action<worldObject> callback)
	{
		onWorldObjectDestroyedCB -= callback;
	}
	public void updateAdjacentTiles(tile t) // update adjacent tiles AND this tile
	{
		t.update();
		t.getAdjacent( 1,  0).update();
		t.getAdjacent(-1,  0).update();
		t.getAdjacent( 0,  1).update();
		t.getAdjacent( 0, -1).update();
	}
}
