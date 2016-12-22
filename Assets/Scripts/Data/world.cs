using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class world
{
	Dictionary<Vector2, tile> worldTiles;

	public character mainCharacter { get; protected set; }

	Action<worldObject> onWorldObjectCreatedCB;
	Action<worldObject> onWorldObjectDestroyedCB;

	public world()
	{

	}
	public void setupWorld(int worldWidth, int worldHeight)
	{
		this.worldTiles = new Dictionary<Vector2, tile>();
		this.mainCharacter = new character(new Vector2(5, 5), "Dave", 0.05f, this); // FIXME this should be loaded somewhere else, it should have easy to edit vars
		for (int x = 0; x < worldWidth; x++)
		{
			for (int y = 0; y < worldHeight; y++)
			{
				tile t = new tile();
				this.worldTiles.Add(new Vector2(x, y), t);
				this.placeTile("grass_0", new Vector2(x, y));
			}
		}
	}
	public tile getTileAt(int x, int y)
	{
		if (x >= 0 && y >= 0)
		{
			Vector2 tmp = new Vector2(x, y);
			return this.worldTiles[tmp];
		}
		else
		{
			Debug.Log(x + " " + y);
			return null;
		}
	}
	public worldObject getWorldObjectAt(int x, int y)
	{
		if (x >= 0 && y >= 0)
		{
			Vector2 tmp = new Vector2(x, y);
			return this.worldTiles[tmp].childObject;
		}
		else
		{
			return null;
		}
	}
	public tile placeTile(string tileType, Vector2 pos)
	{
		if (tileXMLManager.instance.tileProtos.ContainsKey(tileType))
		{
			tile t = tile.placeInstance(tileXMLManager.instance.tileProtos[tileType], pos, this);
			return t;
		}
		else
		{
			Debug.LogError("No tile prototype of type: " + tileType);
			return null;
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
			Debug.LogError("world::placeWorldOject: No object of type " + objectType);
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
