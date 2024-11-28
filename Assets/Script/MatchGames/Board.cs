using UnityEngine;

public class Board:ScriptableObject
{
	public int Width;
	public int Height;
	public GameObject bgCode;
	
	public Coordinate[,] Coordinates;
	
	public Board(int width, int height)
	{
		Width = width;
		Height = height;
		
		Coordinates = new Coordinate[width, height];
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Coordinates![i, j] = new Coordinate(i, j, null);
			}
			
		}
	}

}

