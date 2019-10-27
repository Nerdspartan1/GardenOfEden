using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(LevelManager))]
[RequireComponent(typeof(PropsManager))]
[RequireComponent(typeof(TileGenerator))]
public class MapGenerator : MonoBehaviour
{
	public enum CellType
	{
		Clear = 0,
		Wall = 1
	}

	public GameObject Player;
	public GameObject Monster;

	public int MapSize;
	public int NumberOfSteps;

	[Range(0, 1)]
	public float Corridorness = 0.4f;

	public int NumberOfProps = 30;

	private Grid _mapGrid;
	private NavMeshSurface _navSurface;
	private LevelManager _levelManager;
	private PropsManager _propsManager;
	private TileGenerator _tileGenerator;
	private Vector3 _mapOffset;

	List<List<CellType>> Map;
 
    void Start()
    {
		_mapGrid = GetComponent<Grid>();
		_navSurface = GetComponent<NavMeshSurface>();
		_levelManager = GetComponent<LevelManager>();
		_propsManager = GetComponent<PropsManager>();
		_tileGenerator = GetComponent<TileGenerator>();
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
		for(int i = -1; i <= 1; ++i)
			for(int j = -1; j <= 1; ++j)
				Map[roamerPos.x+i][roamerPos.y+j] = CellType.Clear;

		for (int k = 0; k < NumberOfSteps; ++k)
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
		_tileGenerator.TileMap(Map);

		List<Vector2Int> occupiedCells = new List<Vector2Int>();
		occupiedCells.Add(new Vector2Int(MapSize / 2, MapSize / 2)); //center of the map, where the monolith is

		//Place the player
		Vector2Int playerCell = GetRandomCellPositionInLevel();
		Player.transform.position = CellToWorld(playerCell) + 2f*Vector3.up;
		occupiedCells.Add(playerCell);

		//Place the entity at least one third of the map size away from the player
		Vector2Int monsterCell;
		do
		{
			monsterCell = GetRandomCellPositionInLevel();
		} while ((monsterCell - playerCell).sqrMagnitude < MapSize * MapSize / 9);

		Monster.GetComponent<NavMeshAgent>().Warp(CellToWorld(monsterCell));

		//Place the collectibles
		for (int i = 0; i < _propsManager.Collectibles.Length; i++)
		{
			Vector2Int pos;
			do
			{
				pos = GetRandomCellPositionInLevel();
			} while (occupiedCells.Exists(v => v == pos));
			occupiedCells.Add(pos);
			var collectible = Instantiate(_propsManager.Collectibles[i], CellToWorld(pos), Quaternion.identity, transform).GetComponent<Collectible>();
			collectible.Player = Player;
			collectible.CollectiblePopup = _propsManager.CollectiblePopup;
		}

		//Place the props
		for (int i=0; i < NumberOfProps; i++)
		{
			Vector2Int pos;
			int k = 0; //number of iterations
			do
			{
				pos = GetRandomCellPositionInLevel();
				k++;
			} while (occupiedCells.Exists(v => v == pos) && k<1000);
			if (k == 1000) continue;
			occupiedCells.Add(pos);
			Vector3 randomOffset = Vector3.ProjectOnPlane(0.7f * Random.onUnitSphere * _mapGrid.cellSize.x/2, Vector3.up);
			Instantiate(_propsManager.Props[Random.Range(0,_propsManager.Props.Length)], CellToWorld(pos) + randomOffset, Quaternion.Euler(0,Random.Range(0f,360f),0), transform);
		}
		//Bake the navmesh
		_navSurface.BuildNavMesh();

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

	public Vector3 CellToWorld(Vector2Int cellPos)
	{
		return _mapGrid.CellToWorld(new Vector3Int(cellPos.x, 0, cellPos.y)) - _mapOffset;
	}
}
