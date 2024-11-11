using System;
using UnityEngine;

public class Coordinate
{
	public int X;
	public int Y;
	
	public Tile? Tile;

	public Coordinate(int x, int y, Tile? tile)
	{
		X = x;
		Y = y;
		Tile = tile;
	}
	
	public Tile? getTile()
	{
		return Tile;
	}

	public void setTile(Tile? tile)
	{
		Tile = tile;
	}
}
