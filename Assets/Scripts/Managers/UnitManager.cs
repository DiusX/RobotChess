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
    private List<BaseBuilding> _playerSpawnedBuildings, _enemySpawnedBuildings;
    private BaseRobot _playerSpawnedRobot, _enemySpawnedRobot;

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
        _playerSpawnedBuildings = new List<BaseBuilding>();
        _enemySpawnedBuildings = new List<BaseBuilding>();
    }

    /// <summary>
    /// <para>Spawns a player building onto a given tile.</para>
    /// Once the building is spawned, gameState will be changed to enemy building spawn. <br />
    /// If the enemy has already spawned all of their buildings, gameState will instead be changed to spawn player robot. <br />
    /// In the case that this building spawn causes the opponent not to be able to spawn any further buildings, while being a building down,
    /// the spawned building will instead be destroyed and gameState changed to spawn player robot.
    /// </summary>
    public void SpawnPlayerBuilding(Tile tile)
    {
        //Debug.Log("Player Building Spawn");
        if (_playerBuildingCount < _buildingCount && GridManager.Instance.HasPlaceableTiles(Faction.Player))
        {
            GridManager.Instance.ReducePlaceableTiles(tile.gameObject.transform.position);
            var spawnedPlayerBuilding = Instantiate(_playerBuilding);
            spawnedPlayerBuilding.GetComponent<SpriteRenderer>().sprite = _playerBuildingSprite;            
            tile.SetUnit(spawnedPlayerBuilding);

            _playerSpawnedBuildings.Add(spawnedPlayerBuilding);

            bool opponentCanPlace = GridManager.Instance.HasPlaceableTiles(Faction.Enemy);
            if (!opponentCanPlace)
            {
                if (_playerBuildingCount == _enemyBuildingCount)
                {
                    //TODO: Initiate spacerock destroy building...
                    Debug.Log("PLAYER BUILDING DESTROYED BY ASTROID");
                    tile.CaptureBuilding(Faction.Player); //Or BreakTileOpen

                    _playerSpawnedBuildings.Remove(spawnedPlayerBuilding);
                    GameManager.Instance.ChangeState(GameState.SpawnPlayerRobot);
                }
                else
                {
                    _playerBuildingCount++;
                    GameManager.Instance.ChangeState(GameState.SpawnEnemyRobot);
                }                
            }            
            else
            {
                _playerBuildingCount++;
                GameManager.Instance.ChangeState(GameState.SpawnEnemyBuilding);
            }            
        }
        else { 
            GameManager.Instance.ChangeState(GameState.SpawnPlayerRobot);
        }

    }

    /// <summary>
    /// <para>Spawns an enemy building onto a given tile.</para>
    /// Once the building is spawned, gameState will be changed to spawn player building. <br />
    /// If the player has already spawned all of their buildings, gameState will instead be changed to spawn enemy robot. <br />
    /// In the case that this building spawn causes the opponent not to be able to spawn any further buildings, while being a building down,
    /// the spawned building will instead be destroyed and gameState changed to spawn enemy robot.
    /// </summary>
    public void SpawnEnemyBuilding(Tile tile)
    {
        //Debug.Log("Enemy Building Spawn");
        if (_enemyBuildingCount < _buildingCount && GridManager.Instance.HasPlaceableTiles(Faction.Enemy))
        {
            /*var randomPrefab = GetRandomUnit<BaseBuilding>(Faction.Enemy);
            var spawnedEnemyBuilding = Instantiate(randomPrefab);*/

            GridManager.Instance.ReducePlaceableTiles(tile.gameObject.transform.position);
            var spawnedEnemyBuilding = Instantiate(_enemyBuilding);
            spawnedEnemyBuilding.GetComponent<SpriteRenderer>().sprite = _enemyBuildingSprite;
            tile.SetUnit(spawnedEnemyBuilding);

            _enemySpawnedBuildings.Add(spawnedEnemyBuilding);

            bool opponentCanPlace = GridManager.Instance.HasPlaceableTiles(Faction.Player);
            if (!opponentCanPlace)
            {
                if (_playerBuildingCount == _enemyBuildingCount)
                {
                    //TODO: Initiate spacerock destroy building...
                    Debug.Log("ENEMY BUILDING DESTROYED BY ASTROID");
                    tile.CaptureBuilding(Faction.Enemy); //Or BreakTileOpen

                    _enemySpawnedBuildings.Remove(spawnedEnemyBuilding);
                    GameManager.Instance.ChangeState(GameState.SpawnEnemyRobot);
                }
                else
                {
                    _enemyBuildingCount++;
                    GameManager.Instance.ChangeState(GameState.SpawnPlayerRobot);
                }
            }
            else
            {
                _enemyBuildingCount++;
                GameManager.Instance.ChangeState(GameState.SpawnPlayerBuilding);
            }
        }
        else
        {
            GameManager.Instance.ChangeState(GameState.SpawnEnemyRobot);
        }
    }

    /// <summary>
    /// <para>Spawns a player robot onto a given tile.</para>
    /// Once the robot is spawned, gameState will be changed to enemy robot spawn. <br />
    /// If enemy robot was already spawned, gameState will instead be changed to player turn.
    /// </summary>
    public void SpawnPlayerRobot(Tile tile)
    {
        //Debug.Log("Player Spawn");
        var spawnedPlayerRobot = Instantiate(_playerRobot);
        spawnedPlayerRobot.GetComponent<SpriteRenderer>().sprite = _playerRobotSprite;
        tile.SetUnit(spawnedPlayerRobot);
        PlayerController.Instance.InitPlayer(spawnedPlayerRobot);
        _playerSpawnedRobot = spawnedPlayerRobot;
        _playerSpawned = true;

        if (!_enemySpawned)
        {
            GameManager.Instance.ChangeState(GameState.SpawnEnemyRobot);
        }
        else
        {
            GameManager.Instance.ChangeState(GameState.PlayerTurn);
        }
    }

    /// <summary>
    /// <para>Spawns an enemy robot onto a given tile.</para>
    /// Once the robot is spawned, gameState will be changed to player robot spawn. <br />
    /// If player robot was already spawned, gameState will instead be changed to enemy turn.
    /// </summary>
    public void SpawnEnemyRobot(Tile tile)
    {
        //Debug.Log("Enemy Spawn");
        var spawnedEnemyRobot = Instantiate(_enemyRobot);
        spawnedEnemyRobot.GetComponent<SpriteRenderer>().sprite = _enemyRobotSprite;
        tile.SetUnit(spawnedEnemyRobot);
        PlayerController.Instance.InitEnemy(spawnedEnemyRobot);
        _enemySpawnedRobot = spawnedEnemyRobot;
        _enemySpawned = true;

        if (!_playerSpawned)
        {
            GameManager.Instance.ChangeState(GameState.SpawnPlayerRobot);
        }
        else
        {
            GameManager.Instance.ChangeState(GameState.EnemyTurn);
        }
    }

    /*private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)_units.Where(u => u.Faction == faction && u.UnitPrefab is T).OrderBy(O => Random.value).First().UnitPrefab;
    }*/

    public void ClearShotsOnRobot(Faction faction)
    {
        if (faction == Faction.Player)
        {
            _playerSpawnedRobot.ClearShot();
        }
        else
        {
            _enemySpawnedRobot.ClearShot();
        }
    }

    public void ClearShotsOnBuildings(Faction faction)
    {
        if (faction == Faction.Player)
        {
            foreach(BaseBuilding building in _playerSpawnedBuildings)
                building.ClearShot();
        }
        else
        {
            foreach (BaseBuilding building in _enemySpawnedBuildings)
                building.ClearShot();
        }
    }
}
