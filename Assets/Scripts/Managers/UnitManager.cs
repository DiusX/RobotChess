using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class UnitManager : MonoBehaviour
{//Based on Youtube tutorials from @Tarodev

    [SerializeField] private int _buildingCount;
    [SerializeField] private BaseBuilding _playerBuilding, _enemyBuilding;
    [SerializeField] private BaseRobot _playerRobot, _enemyRobot;
    public int UnitCount => 2 * _buildingCount + 2;

    /// <summary>
    /// This Singleton class manages the spawning and placements of all units at the start of the game
    /// </summary>
    /// TODO: Adjust class to be server code
    public static UnitManager Instance;

    /*private List<ScriptableUnit> _units;*/
    private int _playerBuildingCount, _enemyBuildingCount;
    private bool _playerSpawned, _enemySpawned;
    private Sprite _playerRobotSprite, _enemyRobotSprite, _playerBuildingSprite, _enemyBuildingSprite;

    private void Awake()
    {
        Instance = this;        
    }

    public void Start()
    {
        /*_units = Resources.LoadAll<ScriptableUnit>("Units").ToList();*/
        _playerBuildingCount = 0; _enemyBuildingCount = 0;
        _playerSpawned = false; _enemySpawned = false;
        _playerRobotSprite = SpriteManager.Instance.GetPlayerRobotSprite();
        _enemyRobotSprite = SpriteManager.Instance.GetEnemyRobotSprite();
        _playerBuildingSprite = SpriteManager.Instance.GetPlayerBuildingSprite();
        _enemyBuildingSprite = SpriteManager.Instance.GetEnemyBuildingSprite();
    }

    /// <summary>
    /// Spawns a player building onto a valid tile. <br />
    /// Once the building is spawned, gameState will be changed to enemy building spawn. <br />
    /// When all buildings have already been spawned, gameState will instead be changed to spawning the player robot.
    /// </summary>
    /// TODO: Rework to limit options available and to allow player to select.
    public void SpawnPlayerBuilding(Tile tile)
    {
        Debug.Log("Player Building Spawn");
        if (_playerBuildingCount < _buildingCount && GridManager.Instance.HasPlaceableTiles(Faction.Player))
        {
            GridManager.Instance.ReducePlaceableTiles(tile.gameObject.transform.position);
            var spawnedPlayerBuilding = Instantiate(_playerBuilding);
            spawnedPlayerBuilding.GetComponent<SpriteRenderer>().sprite = _playerBuildingSprite;            
            tile.SetUnit(spawnedPlayerBuilding);

            if (!GridManager.Instance.HasPlaceableTiles(Faction.Enemy) && _playerBuildingCount == _enemyBuildingCount)
            {
                //TODO: Initiate spacerock destroy building...
                tile.CaptureBuilding(Faction.Player); //Or BreakTileOpen
                GameManager.Instance.ChangeState(GameState.SpawnPlayerRobot);
                return;
            }
            _playerBuildingCount++;            

            GameManager.Instance.ChangeState(GameState.SpawnEnemyBuilding);
        }
        else { 
            GameManager.Instance.ChangeState(GameState.SpawnPlayerRobot);
        }

    }

    /// <summary>
    /// Spawns an enemy building onto a valid tile. <br />
    /// Once the building is spawned, gameState will be changed to player building spawn. <br />
    /// When all buildings have already been spawned, gameState will instead be changed to spawning the enemy robot.
    /// </summary>
    /// TODO: Rework to limit options available and to allow enemy to select (local for AI; server for enemy).
    public void SpawnEnemyBuilding(Tile tile)
    {
        Debug.Log("Enemy Building Spawn");
        if (_enemyBuildingCount < _buildingCount && GridManager.Instance.HasPlaceableTiles(Faction.Enemy))
        {
            /*var randomPrefab = GetRandomUnit<BaseBuilding>(Faction.Enemy);
            var spawnedEnemyBuilding = Instantiate(randomPrefab);*/

            GridManager.Instance.ReducePlaceableTiles(tile.gameObject.transform.position);
            var spawnedEnemyBuilding = Instantiate(_enemyBuilding);
            spawnedEnemyBuilding.GetComponent<SpriteRenderer>().sprite = _enemyBuildingSprite;
            tile.SetUnit(spawnedEnemyBuilding);

            if (!GridManager.Instance.HasPlaceableTiles(Faction.Player) && _playerBuildingCount == _enemyBuildingCount)
            {
                //TODO: Initiate spacerock destroy building...
                tile.CaptureBuilding(Faction.Enemy); //Or BreakTileOpen
                GameManager.Instance.ChangeState(GameState.SpawnEnemyRobot);
                return;
            }            
            _enemyBuildingCount++;            

            GameManager.Instance.ChangeState(GameState.SpawnPlayerBuilding);
        }
        else
        {
            GameManager.Instance.ChangeState(GameState.SpawnEnemyRobot);
        }
    }

    /// <summary>
    /// Spawns a player robot onto a valid tile. <br />
    /// Once the robot is spawned, gameState will be changed to enemy robot spawn. <br />
    /// If player robot was already spawned, gameState will instead be changed to enemy turn.
    /// </summary>
    /// TODO: Rework to limit options available and to allow player to select.
    public void SpawnPlayerRobot(Tile tile)
    {
        Debug.Log("Player Spawn");
        if (!_playerSpawned)
        {
            /*var randomPrefab = GetRandomUnit<BaseRobot>(Faction.Player);
            var spawnedPlayerRobot = Instantiate(randomPrefab);*/

            var spawnedPlayerRobot = Instantiate(_playerRobot);
            spawnedPlayerRobot.GetComponent<SpriteRenderer>().sprite = _playerRobotSprite;
            tile.SetUnit(spawnedPlayerRobot);
            PlayerController.Instance.InitPlayer(spawnedPlayerRobot);
            _playerSpawned = true;

            GameManager.Instance.ChangeState(GameState.SpawnEnemyRobot);
        }
        else
        {
            GameManager.Instance.ChangeState(GameState.EnemyTurn);
        }

    }

    /// <summary>
    /// Spawns an enemy robot onto a valid tile. <br />
    /// Once the robot is spawned, gameState will be changed to player robot spawn. <br />
    /// If enemy robot was already spawned, gameState will instead be changed to player turn.
    /// </summary>
    /// TODO: Rework to limit options available and to allow enemy to select (local for AI; server for enemy).
    public void SpawnEnemyRobot(Tile tile)
    {
        Debug.Log("Enemy Spawn");
        if (!_enemySpawned)
        {
            /*var randomPrefab = GetRandomUnit<BaseRobot>(Faction.Enemy);
            var spawnedEnemyRobot = Instantiate(randomPrefab);*/

            var spawnedEnemyRobot = Instantiate(_enemyRobot);
            spawnedEnemyRobot.GetComponent<SpriteRenderer>().sprite = _enemyRobotSprite;
            tile.SetUnit(spawnedEnemyRobot);
            PlayerController.Instance.InitEnemy(spawnedEnemyRobot);
            _enemySpawned = true;

            GameManager.Instance.ChangeState(GameState.SpawnPlayerRobot);
        }
        else
        {
            GameManager.Instance.ChangeState(GameState.PlayerTurn);
        }
    }

    /*private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)_units.Where(u => u.Faction == faction && u.UnitPrefab is T).OrderBy(O => Random.value).First().UnitPrefab;
    }*/
}
