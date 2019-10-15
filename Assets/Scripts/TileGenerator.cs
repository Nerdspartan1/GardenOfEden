using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CellType = MapGenerator.CellType;

public class TileGenerator : MonoBehaviour
{

	private MapGenerator _mapGenerator;
	public Mesh mesh;

	public Mesh Tile0;
	public Mesh Tile1e	;
	public Mesh Tile1s	;
	public Mesh Tile1w	;
	public Mesh Tile1n	;
	public Mesh Tile2es	;
	public Mesh Tile2ew	;
	public Mesh Tile2en	;
	public Mesh Tile2sw	;
	public Mesh Tile2sn	;
	public Mesh Tile2wn	;
	public Mesh Tile3esw;
	public Mesh Tile3swn;
	public Mesh Tile3wne;
	public Mesh Tile3nes;
	public Mesh Tile4;  

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
					var wall = Instantiate(_mapGenerator.WallPrefab, _mapGenerator.CellToWorld(new Vector2Int(x, z)), Quaternion.identity, transform);
					Neighbors neighbors = Neighbors.None;

					if (x == _mapGenerator.MapSize - 1 || map[x + 1][z] == CellType.Wall) neighbors |= Neighbors.East; 
					if (x == 0 || map[x - 1][z] == CellType.Wall)						  neighbors |= Neighbors.West;
					if (z == 0 || map[x][z - 1] == CellType.Wall)						  neighbors |= Neighbors.South;
					if (z == _mapGenerator.MapSize - 1 || map[x][z + 1] == CellType.Wall) neighbors |= Neighbors.North;

					Debug.Log(neighbors);


					wall.GetComponent<MeshFilter>().sharedMesh = GetTile(neighbors);
				}
			}
		}
	}

	private Mesh GetTile(Neighbors neighbors)
	{
		switch (neighbors)
		{
			case Neighbors.East:
				return Tile1e;
			case Neighbors.South:
				return Tile1s;
			case Neighbors.West:
				return Tile1w;
			case Neighbors.North:
				return Tile1n;

			case Neighbors.East | Neighbors.South:
				return Tile2es;
			case Neighbors.East | Neighbors.West:
				return Tile2ew;
			case Neighbors.East | Neighbors.North:
				return Tile2en;
			case Neighbors.South | Neighbors.West:
				return Tile2sw;
			case Neighbors.South | Neighbors.North:
				return Tile2sn;
			case Neighbors.West | Neighbors.North:
				return Tile2wn;

			case Neighbors.East | Neighbors.South | Neighbors.West:
				return Tile3esw;
			case Neighbors.South | Neighbors.West | Neighbors.North:
				return Tile3swn;
			case Neighbors.West | Neighbors.North | Neighbors.East:
				return Tile3wne;
			case Neighbors.North | Neighbors.East | Neighbors.South:
				return Tile3nes;

			case Neighbors.All:
				return Tile4;

			default:
				return Tile0;
		}

	}

}
