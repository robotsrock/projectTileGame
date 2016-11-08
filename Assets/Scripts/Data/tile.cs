using UnityEngine;
using System.Collections;
using System;

public enum tileType
{
	empty,
	dirt,
	grass
};
public class tile //! REFACTOR use properties!!!!!
{
	tileType tileBase; // the type of the tile eg. dirt, grass, sand
	int tileVariant;   // the type variation eg. blue grass, red dirt, white sand, might have different characteristics than other variants, like moverate

	public Vector2 position { get; protected set; }  // position of the tile in data space
	public worldObject childObject { get; protected set; }

	Action<tile> onSetCB; // call back for setting the type

	public tile(tileType type, int variant, Vector2 position)
	{
		this.tileBase = type;
		this.tileVariant = variant;
		this.position = position;
	}
	public tileType getTileType()
	{
		return this.tileBase;
	}
	public int getTileVariant()
	{
		return this.tileVariant;
	}
	public void setTile(tileType type, int variant)
	{
		this.tileBase = type;
		this.tileVariant = variant;
		if (this.onSetCB != null)
		{
			this.onSetCB(this);
		}
	}
	public void registerSetCallback(Action<tile> callback)
	{
		onSetCB += callback;
	}
	public void unRegisterSetCallback(Action<tile> callback)
	{
		onSetCB -= callback;
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
			Debug.Log("Tile_" + this.position.x + "_" + this.position.y + " already has a world object");
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
}
