using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapGenerator : MonoBehaviour
{
	enum CellType
	{
		Clear,
		Wall
	}

	public Transform ParentObject;

	public Vector2Int MapSize;
	public GameObject WallPrefab;

	public int NumberOfSteps;

	[Range(0, 1)]
	public float Corridorness = 0.4f;
	

	private Grid _mapGrid;
	private NavMeshSurface _navSurface;
	
	List<List<CellType>> Map;
 
    void Start()
    {
		_mapGrid = GetComponent<Grid>();
		_navSurface = ParentObject.GetComponent<NavMeshSurface>();
		Generate();
    }

	public void Generate()
	{
		//Initialize the map
		Map = new List<List<CellType>>(MapSize.x);
		for(int x = 0; x < MapSize.x; ++x)
		{
			Map.Add(new List<CellType>(MapSize.y));
			for(int z = 0; z < MapSize.y; ++z)
			{
				Map[x].Add(CellType.Wall);
			}
		}

		// Roaming algorithm
		// start in the middle
		Vector2Int roamerPos = new Vector2Int(MapSize.x / 2, MapSize.y / 2);
		Vector2Int roamerDir = new Vector2Int(1, 0);
		Map[roamerPos.x][roamerPos.y] = CellType.Clear;

		for(int k = 0; k < NumberOfSteps; ++k)
		{
			// calculate direction
			float rand = Random.Range(0f, 1f);
			if (rand > Corridorness) // the lower the corridorness, the highest the chance of turning
			{
				rand = Random.Range(0f, 1f);
				if(rand < 0.4f)
					roamerDir = new Vector2Int(roamerDir.y, -roamerDir.x); //turn right
				else if(rand < 0.8f)

					roamerDir = new Vector2Int(-roamerDir.y, roamerDir.x); //turn left
				else
					roamerDir *= -1; // turn back
			}

			// flip roamer direction if it goes into the exterior walls
			var nextPos = roamerPos + roamerDir;
			if (nextPos.x < 1 || nextPos.x >= MapSize.x - 1 || nextPos.y < 1 || nextPos.y >= MapSize.y - 1)
				roamerDir *= -1;

			//move roamer
			roamerPos += roamerDir;
			Map[roamerPos.x][roamerPos.y] = CellType.Clear;
		}

		//Fill out the scene
		Vector3 mapOffset = new Vector3(MapSize.x / 2 * _mapGrid.cellSize.x, 0, MapSize.y / 2 * _mapGrid.cellSize.z);
		for (int x = 0; x < MapSize.x; ++x)
		{
			for (int z = 0; z < MapSize.y; ++z)
			{
				if(Map[x][z] == CellType.Wall)
					Instantiate(WallPrefab, _mapGrid.CellToWorld(new Vector3Int(x, 0, z)) - mapOffset, Quaternion.identity,ParentObject);
			}
		}

		//Bake the navmesh
		_navSurface.BuildNavMesh();
	}

}
