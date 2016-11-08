using UnityEngine;
using System.Collections;
using System;

public enum tileType
{
	empty,
	dirt,
	grass
};
public class tile
{
	tileType tileBase; // the type of the tile eg. dirt, grass, sand
	int tileVariant;   // the type variation eg. blue grass, red dirt, white sand, might have different characteristics than other variants, like moverate

	Vector2 position;  // position of the tile in data space
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
}
