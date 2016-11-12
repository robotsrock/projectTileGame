using UnityEngine;
using System.Collections;
using System;

public class worldObject
{
	// TODO implement rotation
	// TODO implement larger objects
	public string objectType { get; protected set; }
	public float movementCost { get; protected set; }
	public tile baseTile { get; protected set; }
	public uint flags { get; protected set; }			//! FLAGS:   3: left WO   2: down WO   1: right WO   0: up WO
	int width;
	int height;

	Action<worldObject> onChangeCB; // TODO implement this in other files

	protected worldObject()
	{
	}

	public static worldObject createPrototype(string objectType, float movementCost, int width, int height)
	{
		worldObject obj = new worldObject();

		obj.objectType = objectType;
		obj.movementCost = movementCost;
		obj.width = width;
		obj.height = height;

		return obj;
	}

	public static worldObject placeInstance(worldObject proto, tile baseTile)
	{
		worldObject obj = new worldObject();

		obj.objectType = proto.objectType;
		obj.movementCost = proto.movementCost;
		obj.width = proto.width;
		obj.height = proto.height;
		obj.baseTile = baseTile;
		obj.flags = 0;

		// FIXME maybe we can have multiple tile objects?
		if (!baseTile.placeObject(obj))
		{
			// the placement failed
			return null;
		}
		return obj;

	}
	public static bool destroyInstance(tile baseTile)
	{
		if(baseTile.destroyObject())
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	public void registerOnChangedCB(Action<worldObject> callback)
	{
		onChangeCB += callback;
	}
	public void unRegisterOnChangedCB(Action<worldObject> callback)
	{
		onChangeCB -= callback;
	}
	public void update()
	{
		// Set the flags
		this.setFlags();

		if (this.onChangeCB != null)
		{
			this.onChangeCB(this);
		}
	}
	public void setFlags()
	{
		if (this.baseTile.getAdjacentObjectType( 0,  1) == this.objectType) // | with a 1 sets, & with a 0 clears
		{
			this.flags |= 1;	// xxxx | 0001
        }
		else
		{
			this.flags &= 14;	// xxxx & 1110
		}
		if (this.baseTile.getAdjacentObjectType( 1,  0) == this.objectType)
		{
			this.flags |= 2;    // xxxx | 0010
		}
		else
		{
			this.flags &= 13;   // xxxx & 1101
		}
		if (this.baseTile.getAdjacentObjectType( 0, -1) == this.objectType)
		{
			this.flags |= 4;    // xxxx | 0100
		}
		else
		{
			this.flags &= 11;   // xxxx & 1011
		}
		if (this.baseTile.getAdjacentObjectType(-1,  0) == this.objectType)
		{
			this.flags |= 8;    // xxxx | 1000
		}
		else
		{
			this.flags &= 7;   // xxxx & 0111
		}
	}
}
