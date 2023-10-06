using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
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
    public static UnitManager Instance;

    /*private List<ScriptableUnit> _units;*/
    private int _playerBuildingCount, _enemyBuildingCount;
    private bool _playerSpawned, _enemySpawned;
    private List<BaseBuilding> _playerSpawnedBuildings, _enemySpawnedBuildings;
    private BaseRobot _playerSpawnedRobot, _enemySpawnedRobot;

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        _playerBuildingCount = 0; _enemyBuildingCount = 0;
        _playerSpawned = false; _enemySpawned = false;
        _playerSpawnedBuildings = new List<BaseBuilding>();
        _enemySpawnedBuildings = new List<BaseBuilding>();
    }

    public void SpawnUnit(Tile tile)
    {
        switch (GameManager.Instance.Gamestate.Value)
        {
            case (GameState.SpawnPlayerBuilding): SpawnPlayerBuilding(tile); break;
            case (GameState.SpawnEnemyBuilding): SpawnEnemyBuilding(tile); break;
            case (GameState.SpawnPlayerRobot): SpawnPlayerRobot(tile); break;
            case (GameState.SpawnEnemyRobot): SpawnEnemyRobot(tile); break;
        }
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
        Debug.Log("Player Building Spawn");
        if (_playerBuildingCount < _buildingCount && GridManager.Instance.HasPlaceableTiles(Faction.Player))
        {
            GridManager.Instance.ReducePlaceableTiles(tile.transform.position);
            var spawnedPlayerBuilding = Instantiate(_playerBuilding);
            spawnedPlayerBuilding.GetComponent<NetworkObject>().Spawn(true);
            spawnedPlayerBuilding.InitClientRpc();
            tile.SetUnit(spawnedPlayerBuilding);
            spawnedPlayerBuilding.SetUnitOnTileClientRpc(tile.transform.position);

            _playerSpawnedBuildings.Add(spawnedPlayerBuilding); //CHECK

            bool opponentCanPlace = GridManager.Instance.HasPlaceableTiles(Faction.Enemy) && _enemyBuildingCount != _buildingCount;
            if (!opponentCanPlace)
            {
                if (_playerBuildingCount == _enemyBuildingCount)
                {
                    //TODO: Initiate spacerock destroy building...
                    Debug.Log("PLAYER BUILDING DESTROYED BY ASTROID");
                    tile.CaptureBuilding(Faction.Player); //Or BreakTileOpen
                    tile.CaptureBuildingClientRpc(Faction.Player);

                    _playerSpawnedBuildings.Remove(spawnedPlayerBuilding);
                    GameManager.Instance.ChangeStateServerRpc(GameState.SpawnPlayerRobot);
                }
                else
                {
                    _playerBuildingCount++;
                    GameManager.Instance.ChangeStateServerRpc(GameState.SpawnEnemyRobot);
                }
            }
            else
            {
                _playerBuildingCount++;
                GameManager.Instance.ChangeStateServerRpc(GameState.SpawnEnemyBuilding);
            }
        }
        else
        {
            GameManager.Instance.ChangeStateServerRpc(GameState.SpawnPlayerRobot);
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
        Debug.Log("Enemy Building Spawn");
        if (_enemyBuildingCount < _buildingCount && GridManager.Instance.HasPlaceableTiles(Faction.Enemy))
        {

            GridManager.Instance.ReducePlaceableTiles(tile.transform.position);
            var spawnedEnemyBuilding = Instantiate(_enemyBuilding);
            spawnedEnemyBuilding.GetComponent<NetworkObject>().Spawn(true);
            spawnedEnemyBuilding.InitClientRpc();
            tile.SetUnit(spawnedEnemyBuilding);
            spawnedEnemyBuilding.SetUnitOnTileClientRpc(tile.transform.position);

            _enemySpawnedBuildings.Add(spawnedEnemyBuilding);

            bool opponentCanPlace = GridManager.Instance.HasPlaceableTiles(Faction.Player) && _playerBuildingCount != _buildingCount;
            if (!opponentCanPlace)
            {
                if (_playerBuildingCount == _enemyBuildingCount)
                {
                    //TODO: Initiate spacerock destroy building...
                    Debug.Log("ENEMY BUILDING DESTROYED BY ASTROID");
                    tile.CaptureBuilding(Faction.Enemy); //Or BreakTileOpen
                    tile.CaptureBuildingClientRpc(Faction.Enemy);

                    _enemySpawnedBuildings.Remove(spawnedEnemyBuilding);
                    GameManager.Instance.ChangeStateServerRpc(GameState.SpawnEnemyRobot);
                }
                else
                {
                    _enemyBuildingCount++;
                    GameManager.Instance.ChangeStateServerRpc(GameState.SpawnPlayerRobot);
                }
            }
            else
            {
                _enemyBuildingCount++;
                GameManager.Instance.ChangeStateServerRpc(GameState.SpawnPlayerBuilding);
            }
        }
        else
        {
            GameManager.Instance.ChangeStateServerRpc(GameState.SpawnEnemyRobot);
        }
    }

    /// <summary>
    /// <para>Spawns a player robot onto a given tile.</para>
    /// Once the robot is spawned, gameState will be changed to enemy robot spawn. <br />
    /// If enemy robot was already spawned, gameState will instead be changed to player turn.
    /// </summary>
    public void SpawnPlayerRobot(Tile tile)
    {
        Debug.Log("Player Spawn");
        var spawnedPlayerRobot = Instantiate(_playerRobot);
        spawnedPlayerRobot.GetComponent<NetworkObject>().Spawn(true);
        spawnedPlayerRobot.InitClientRpc();
        tile.SetUnit(spawnedPlayerRobot);
        spawnedPlayerRobot.SetUnitOnTileClientRpc(tile.transform.position);

        RobotController.Instance.InitPlayerOnServer(spawnedPlayerRobot); //CHECK
        _playerSpawnedRobot = spawnedPlayerRobot;
        _playerSpawned = true;

        if (!_enemySpawned)
        {
            GameManager.Instance.ChangeStateServerRpc(GameState.SpawnEnemyRobot);
        }
        else
        {
            GameManager.Instance.ChangeStateServerRpc(GameState.PlayerTurn);
        }
    }

    /// <summary>
    /// <para>Spawns an enemy robot onto a given tile.</para>
    /// Once the robot is spawned, gameState will be changed to player robot spawn. <br />
    /// If player robot was already spawned, gameState will instead be changed to enemy turn.
    /// </summary>
    public void SpawnEnemyRobot(Tile tile)
    {
        Debug.Log("Enemy Spawn");
        var spawnedEnemyRobot = Instantiate(_enemyRobot);
        spawnedEnemyRobot.GetComponent<NetworkObject>().Spawn(true);
        spawnedEnemyRobot.InitClientRpc();
        tile.SetUnit(spawnedEnemyRobot);
        spawnedEnemyRobot.SetUnitOnTileClientRpc(tile.transform.position);

        RobotController.Instance.InitEnemyOnServer(spawnedEnemyRobot); //CHECK
        _enemySpawnedRobot = spawnedEnemyRobot;
        _enemySpawned = true;

        if (!_playerSpawned)
        {
            GameManager.Instance.ChangeStateServerRpc(GameState.SpawnPlayerRobot);
        }
        else
        {
            GameManager.Instance.ChangeStateServerRpc(GameState.EnemyTurn);
        }
    }

    public void ClearShotsOnTurnStart(Faction faction)
    {
        Debug.Log("Clearing shots on start of turn of " + nameof(faction));
        if (faction == Faction.Player)
        {
            _enemySpawnedRobot.ClearShot();
            //_enemySpawnedRobot.ClearShotClientRpc();
            foreach (BaseBuilding building in _playerSpawnedBuildings)
            {
                building.ClearShot();
                //building.ClearShotClientRpc();
            }
        }
        else
        {
            _playerSpawnedRobot.ClearShot();
            //_playerSpawnedRobot.ClearShotClientRpc();
            foreach (BaseBuilding building in _enemySpawnedBuildings)
            {
                building.ClearShot();
                //building.ClearShotClientRpc();
            }
        }
    }

    /// <summary>
    /// Given a direction, gets the direction turned 90deg clockwise.
    /// </summary>
    /// <param name="direction">The direction to turn right from.</param>
    /// <returns>The right-turned direction.</returns>
    public UnitDirection GetRightTurn(UnitDirection direction)
    {
        switch (direction)
        {
            case UnitDirection.North: return UnitDirection.East;
            case UnitDirection.East: return UnitDirection.South;
            case UnitDirection.South: return UnitDirection.West;
            case UnitDirection.West: return UnitDirection.North;
            default: return UnitDirection.South;
        }
    }

    /// <summary>
    /// Given a direction, gets the direction turned 90deg anti-clockwise.
    /// </summary>
    /// <param name="direction">The direction to turn left from.</param>
    /// <returns>The left-turned direction.</returns>
    public UnitDirection GetLeftTurn(UnitDirection direction)
    {
        switch (direction)
        {
            case UnitDirection.North: return UnitDirection.West;
            case UnitDirection.East: return UnitDirection.North;
            case UnitDirection.South: return UnitDirection.East;
            case UnitDirection.West: return UnitDirection.South;
            default: return UnitDirection.South;
        }
    }

    public BaseUnit FindUnitOnPos(Vector2 position)
    {
        Tile tile = TileManager.Instance.GetLocalPlayableTile(position);
        if (tile != null)
        {
            return tile.OccupiedUnit;
        }
        return null;
    }

    public int ReturnPlayerScore()
    {
        return _enemyBuildingCount - _enemySpawnedBuildings.Count;
    }

    public int ReturnEnemyScore()
    {
        return _playerBuildingCount - _playerSpawnedBuildings.Count;
    }

    public bool CheckIsGameOver()
    {
        bool isGameOver = _playerSpawnedBuildings.Count == 0 || _enemySpawnedBuildings.Count == 0;
        if (isGameOver)
        {
            GameManager.Instance.ChangeStateServerRpc(GameState.GameOver);
        }
        return isGameOver;
    }

    public void TimerRanOut(Faction faction)
    {        
        BaseBuilding building = GetRandomBuilding(faction);
        if (faction == Faction.Player && GameManager.Instance.Gamestate.Value == GameState.EnemyTurn) return; //racecondition check
        if (faction == Faction.Enemy && GameManager.Instance.Gamestate.Value == GameState.PlayerTurn) return; //racecondition check
        StartCoroutine(DestroyBuilding(building));
    }

    public void DestroyRandomBuilding(Faction faction)
    {
        DestroyBuilding(GetRandomBuilding(faction));
    }

    public BaseBuilding GetRandomBuilding(Faction faction) {
        if (faction == Faction.Player)
        {
            return _playerSpawnedBuildings[Random.Range(0, _playerSpawnedBuildings.Count - 1)];
        }
        else return _enemySpawnedBuildings[Random.Range(0, _enemySpawnedBuildings.Count - 1)];
    }

    public IEnumerator DestroyBuilding(BaseBuilding building)
    {
        yield return new WaitForSeconds(1);
        if(building.Faction == Faction.Player)
        {
            _playerSpawnedBuildings.Remove(building);
        }
        else _enemySpawnedBuildings.Remove(building);
        Destroy(building);
        CheckIsGameOver();
    }
}
