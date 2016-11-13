using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class tileSpriteController : MonoBehaviour 
{
	Dictionary<tile, GameObject> tileToGO;

	// Use this for initialization
	void Awake () 
	{
		setup();
	}
	void setup() // sets up all our variables
	{
		tileToGO = new Dictionary<tile, GameObject>();
	}
	public void addGO(int x, int y, tile t)
	{
		GameObject tileGO = new GameObject("tile_" + x + "_" + y);
		tileGO.transform.SetParent(this.transform);
		tileGO.transform.position = new Vector3(x, y, 0);
		SpriteRenderer tileSR = tileGO.gameObject.AddComponent<SpriteRenderer>();
		tileSR.sprite = spriteManager.instance.getSprite(t.tileBase, t.tileBase + "_" + t.tileVariant + "_0"); // FIXME hardcoded flag, flags not implemented

		tileToGO.Add(t, tileGO);

		t.registerSetCallback((tile) => { this.onTileChanged(tile, tileGO); });
	}

	//?+ callbacks
	public void onTileChanged(tile t, GameObject tileGO) // when a tile changes type or variant, we update the sprite
	{
		tileGO.GetComponent<SpriteRenderer>().sprite = spriteManager.instance.getSprite(t.tileBase, t.tileBase + "_" + t.tileVariant + "_0");
	}

}
