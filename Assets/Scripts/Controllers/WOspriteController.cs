using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class WOspriteController : MonoBehaviour 
{
	Dictionary<worldObject, GameObject> worldObjects;

	worldController worldCon;

	void Start()
	{
		setup();
	}
	void setup()
	{
		worldObjects = new Dictionary<worldObject, GameObject>();
		worldCon = GameObject.FindGameObjectWithTag("worldController").GetComponent<worldController>();

		if (worldCon.firstWorld == null) // make sure that the world is set up
		{
			worldCon.setupWorld();
		}
		worldCon.firstWorld.registerWorldObjectCreatedCB(onWorldObjectCreated);
		worldCon.firstWorld.registerWorldObjectDestroyedCB(onWorldObjectDestroyed);
	}

	//?+ callbacks
	public void onWorldObjectCreated(worldObject obj)
	{
		// create a GO for the obj
		GameObject objGO = new GameObject();

		worldObjects.Add(obj, objGO); // add it to the list

		objGO.name = obj.objectType + "_" + obj.baseTile.position.x + "_" + obj.baseTile.position.y;
		objGO.transform.SetParent(this.transform);
		objGO.transform.position = new Vector3(obj.baseTile.position.x, obj.baseTile.position.y, 0);
		SpriteRenderer tileSR = objGO.AddComponent<SpriteRenderer>();
		tileSR.sprite = spriteManager.instance.getSprite(obj.objectType, obj.objectType + "_0_0"); // TODO wall variants???
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
			"wall",
			obj.objectType + "_0_" + obj.flags);
	}
}
