using UnityEngine;
using System.Collections;
using System;

public class tile
{
	public string tileType { get; protected set; } // the type of the tile eg. dirt, grass, sand
												   //public string locName { get; protected set; }
	public string baseType { get; protected set; }
	public Vector2 position { get; protected set; }  // position of the tile in data space
	public worldObject childObject { get; protected set; } // TODO load from XML, create prototypes
	public world parentWorld { get; protected set; }

	Action<tile> onSetCB; // call back for setting the type

	public tile()
	{

	}
	public static tile createPrototype(string tileType, string baseType) // TODO implement locName && moveRates for tiles
	{
		tile t = new tile();

		t.tileType = tileType;
		t.baseType = baseType;

		return t;
	}
	public static tile placeInstance(tile proto, Vector2 pos, world parWorld)
	{
		tile t = parWorld.getTileAt((int)pos.x, (int)pos.y);

		t.tileType = proto.tileType;
		t.baseType = proto.baseType;
		t.parentWorld = parWorld;
		t.position = pos;

		if (t.onSetCB != null)
		{
			t.onSetCB(t); // calls the callback
		}
		return t;
	}
	public void registerSetCallback(Action<tile> callback)
	{
		this.onSetCB += callback;
	}
	public void unRegisterSetCallback(Action<tile> callback)
	{
		this.onSetCB -= callback;
	}
	public bool placeObject(worldObject objInstance)
	{
		if (objInstance == null)
		{
			this.childObject = null;
			return false;
		}
		else if (this.childObject == null)
		{
			this.childObject = objInstance;
			return true;
		}
		else if (this.childObject != null)
		{
			//Debug.Log("Tile_" + this.position.x + "_" + this.position.y + " already has a world object");
			return false;
		}
		else
		{
			return false;
		}
	}
	public bool destroyObject()
	{
		if (this.childObject == null)
		{
			return false;
		}
		else
		{
			this.childObject = null;
			return true;
		}
	}
	public void update()
	{
		if (this.childObject != null)
		{
			this.childObject.update();
		}
	}
	public tile getAdjacent(int x, int y) // returns an adjacent tile in x and y direction
	{
		if (x > 1 || y > 1 || x < -1 || y < -1) // if we get a value not in the -1 to 1 range
		{
			Debug.Log("tile::getAdjacent: Not in range");
			return null;
		}
		if (x == y) // TODO implement diagonals
		{
			Debug.Log("tile::getAdjacent: Diagonals not implemented");
			return null;
		}
		return this.parentWorld.getTileAt((int)this.position.x + x, (int)this.position.y + y);
	}
	public string getAdjacentObjectType(int x, int y)
	{
		if (x > 1 || y > 1 || x < -1 || y < -1) // if we get a value not in the -1 to 1 range
		{
			Debug.Log("tile::getAdjacent: Not in range");
			return null;
		}
		if (x == y) // TODO implement diagonals
		{
			Debug.Log("tile::getAdjacentObjectType: Diagonals not implemented");
			return null;
		}
		if (this.parentWorld.getTileAt((int)this.position.x + x, (int)this.position.y + y).childObject != null)
		{
			return this.parentWorld.getTileAt((int)this.position.x + x, (int)this.position.y + y).childObject.objectType;
        }
		return null;
	}
}
