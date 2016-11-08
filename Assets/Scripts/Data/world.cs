using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class world
{
	tile[,] worldTiles;
	character mainCharacter;

	public world()
	{

	}
	public void setupWorld(int worldWidth, int worldHeight)
	{
		this.worldTiles = new tile[worldWidth, worldHeight];
		this.mainCharacter = new character(new Vector2(5, 5), "Dave", 0.05f); // FIXME this should be loaded somewhere else, it should have easy to edit vars
		for (int x = 0; x < worldWidth; x++)
		{
			for (int y = 0; y < worldHeight; y++)
			{
				this.worldTiles[x, y] = new tile(tileType.grass, 0, new Vector2(x, y));
			}
		}
	}
	public tile getTileAt(int x, int y)
	{
		return this.worldTiles[x, y];
	}
	public void setTileAt(int x, int y, tileType type, int variant)
	{
		this.worldTiles[x, y].setTile(type, variant);
	}
	public character getMainCharacter()
	{
		return this.mainCharacter;
	}
}
