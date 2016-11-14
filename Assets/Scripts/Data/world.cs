using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class world // REFACTOR use properties
{
	tile[,] worldTiles;

	public character mainCharacter { get; protected set; }

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
				this.worldTiles[x, y] = new tile("grass", 0, new Vector2(x, y), this);  // create a basic grass tile for all tiles (they wil be changed later)
																						// TODO create a proper world generator, and call it here
			}
		}
	}
	public tile getTileAt(int x, int y)
	{
		if (x >= 0 && y >= 0)
		{
			return this.worldTiles[x, y];
		}
		else
		{
			return null;
		}
	}
	public worldObject getWorldObjectAt(int x, int y)
	{
		if (x >= 0 && y >= 0)
		{
			return this.worldTiles[x, y].childObject;
		}
		else
		{
			return null;
		}
	}
	public void setTileAt(int x, int y, string type, int variant)
	{
		if (x >= 0 && y >= 0)
		{
			this.worldTiles[x, y].setTile(type, variant);
		}
	}
	public void placeWorldObject (string objectType, tile t)
	{
		// TODO make it use rotation and big objects
		if (objectXMLManager.instance.worldObjectProtos.ContainsKey(objectType))
		{
			worldObject obj = worldObject.placeInstance(objectXMLManager.instance.worldObjectProtos[objectType], t);
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
