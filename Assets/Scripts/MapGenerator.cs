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
	public GameObject Player;
	public GameObject Monster;

	public int MapSize;
	public GameObject WallPrefab;

	public int NumberOfSteps;

	[Range(0, 1)]
	public float Corridorness = 0.4f;
	

	private Grid _mapGrid;
	private NavMeshSurface _navSurface;
	private Vector3 _mapOffset;

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
		Map = new List<List<CellType>>(MapSize);
		for(int x = 0; x < MapSize; ++x)
		{
			Map.Add(new List<CellType>(MapSize));
			for(int z = 0; z < MapSize; ++z)
			{
				Map[x].Add(CellType.Wall);
			}
		}

		// Roaming algorithm
		// start in the middle
		Vector2Int roamerPos = new Vector2Int(MapSize / 2, MapSize / 2);
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
			if (nextPos.x < 1 || nextPos.x >= MapSize - 1 || nextPos.y < 1 || nextPos.y >= MapSize - 1)
				roamerDir *= -1;

			//move roamer
			roamerPos += roamerDir;
			Map[roamerPos.x][roamerPos.y] = CellType.Clear;
		}

		//Fill out the scene
		_mapOffset = new Vector3(MapSize / 2 * _mapGrid.cellSize.x, 0, MapSize / 2 * _mapGrid.cellSize.z);
		for (int x = 0; x < MapSize; ++x)
		{
			for (int z = 0; z < MapSize; ++z)
			{
				if(Map[x][z] == CellType.Wall)
					Instantiate(WallPrefab, CellToWorld(new Vector2Int(x,z)), Quaternion.identity,ParentObject);
			}
		}

		//Bake the navmesh
		_navSurface.BuildNavMesh();

		//Place the player
		Vector2Int playerCell = GetRandomCellPositionInLevel();
		Player.transform.position = CellToWorld(playerCell) + 1f*Vector3.up;

		//Place the entity at least one third of the map size away from the player
		Vector2Int monsterCell;
		do
		{
			monsterCell = GetRandomCellPositionInLevel();
		} while ((monsterCell - playerCell).sqrMagnitude < MapSize * MapSize / 9);

		Monster.transform.position = CellToWorld(monsterCell);
		Monster.GetComponent<NavMeshAgent>().enabled = true;
	}

	public Vector2Int GetRandomCellPositionInLevel()
	{
		int x, z;
		do
		{
			x = Random.Range(1, MapSize - 1);
			z = Random.Range(1, MapSize - 1);
		} while (Map[x][z] == CellType.Wall);
		return new Vector2Int(x, z);
	}

	private Vector3 CellToWorld(Vector2Int cellPos)
	{
		return _mapGrid.CellToWorld(new Vector3Int(cellPos.x, 0, cellPos.y)) - _mapOffset;
	}
}
