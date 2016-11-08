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
}
