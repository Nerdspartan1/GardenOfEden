using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CellType = MapGenerator.CellType;

public class TileGenerator : MonoBehaviour
{

	private MapGenerator _mapGenerator;

	public GameObject Tile0;
	public GameObject Tile1;
	public GameObject Tile2_straight;
	public GameObject Tile2_corner;
	public GameObject Tile3;
	public GameObject Tile4;  

	[System.Flags]
	enum Neighbors
	{
		None = 0,
		East = 1,
		South = 2,
		West = 4,
		North = 8,
		All = East | South | West | North
	}

	void Awake()
	{
		_mapGenerator = GetComponent<MapGenerator>();
	}

	public void TileMap(List<List<CellType>> map)
	{
		for (int x = 0; x < _mapGenerator.MapSize; ++x)
		{
			for (int z = 0; z < _mapGenerator.MapSize; ++z)
			{
				if (map[x][z] == CellType.Wall)
				{
					
					Neighbors neighbors = Neighbors.None;

					if (x == _mapGenerator.MapSize - 1 || map[x + 1][z] == CellType.Wall) neighbors |= Neighbors.East; 
					if (x == 0 || map[x - 1][z] == CellType.Wall)						  neighbors |= Neighbors.West;
					if (z == 0 || map[x][z - 1] == CellType.Wall)						  neighbors |= Neighbors.South;
					if (z == _mapGenerator.MapSize - 1 || map[x][z + 1] == CellType.Wall) neighbors |= Neighbors.North;

					var wall = Instantiate(GetTilePrefab(neighbors), _mapGenerator.CellToWorld(new Vector2Int(x, z)), GetTileRotation(neighbors), transform);
				}
			}
		}
	}

	private GameObject GetTilePrefab(Neighbors neighbors)
	{
		switch (neighbors)
		{
			case Neighbors.East:
			case Neighbors.South:
			case Neighbors.West:
			case Neighbors.North:
				return Tile1;

			case Neighbors.East | Neighbors.South:
			case Neighbors.West | Neighbors.North:
			case Neighbors.East | Neighbors.North:
			case Neighbors.South | Neighbors.West:
				return Tile2_corner;

			case Neighbors.South | Neighbors.North:
			case Neighbors.East | Neighbors.West:
				return Tile2_straight;

			case Neighbors.East | Neighbors.South | Neighbors.West:
			case Neighbors.South | Neighbors.West | Neighbors.North:
			case Neighbors.West | Neighbors.North | Neighbors.East:
			case Neighbors.North | Neighbors.East | Neighbors.South:
				return Tile3;

			case Neighbors.All:
				return Tile4;

			default:
				return Tile0;
		}

	}

	private Quaternion GetTileRotation(Neighbors neighbors)
	{
		switch (neighbors)
		{
			case Neighbors.East:
				return Quaternion.identity;
			case Neighbors.South:
				return Quaternion.Euler(0, 90f, 0);
			case Neighbors.West:
				return Quaternion.Euler(0, 180f, 0);
			case Neighbors.North:
				return Quaternion.Euler(0, 270f, 0);

			case Neighbors.East | Neighbors.South:
				return Quaternion.identity;
			case Neighbors.South | Neighbors.West:
				return Quaternion.Euler(0, 90f, 0);
			case Neighbors.West | Neighbors.North:
				return Quaternion.Euler(0, 180f, 0);
			case Neighbors.North | Neighbors.East:
				return Quaternion.Euler(0, 270f, 0);

			case Neighbors.East | Neighbors.West:
				return Quaternion.identity;
			case Neighbors.South | Neighbors.North:
				return Quaternion.Euler(0, 90f, 0);


			case Neighbors.East | Neighbors.South | Neighbors.West:
				return Quaternion.identity;
			case Neighbors.South | Neighbors.West | Neighbors.North:
				return Quaternion.Euler(0, 90f, 0);
			case Neighbors.West | Neighbors.North | Neighbors.East:
				return Quaternion.Euler(0, 180f, 0);
			case Neighbors.North | Neighbors.East | Neighbors.South:
				return Quaternion.Euler(0, 270f, 0);

			default:
				return Quaternion.identity;
		}
	}

}
