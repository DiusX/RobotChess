using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField] private int _buildingCount;
    [SerializeField] private BaseBuilding _playerBuilding, _enemyBuilding;
    [SerializeField] private BaseRobot _playerRobot, _enemyRobot;
    public static UnitManager Instance;

    //private List<ScriptableUnit> _units;
    private int _playerBuildingCount, _enemyBuildingCount;
    private bool _playerSpawned, _enemySpawned;
    private Sprite _playerRobotSprite, _enemyRobotSprite, _playerBuildingSprite, _enemyBuildingSprite;

    private void Awake()
    {
        Instance = this;        
    }

    public void Start()
    {
        //_units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
        _playerBuildingCount = 0; _enemyBuildingCount = 0;
        _playerSpawned = false; _enemySpawned = false;
        _playerRobotSprite = SpriteManager.Instance.GetPlayerRobotSprite();
        _enemyRobotSprite = SpriteManager.Instance.GetEnemyRobotSprite();
        _playerBuildingSprite = SpriteManager.Instance.GetPlayerBuildingSprite();
        _enemyBuildingSprite = SpriteManager.Instance.GetEnemyBuildingSprite();
    }

    public void SpawnPlayerBuilding()
    {
        Debug.Log("Player Building Spawn");
        if (_playerBuildingCount < _buildingCount)
        {
            /*var randomPrefab = GetRandomUnit<BaseBuilding>(Faction.Player);
            var spawnedPlayerBuilding = Instantiate(randomPrefab);*/

            var spawnedPlayerBuilding = Instantiate(_playerBuilding);
            spawnedPlayerBuilding.GetComponent<SpriteRenderer>().sprite = _playerBuildingSprite;
            var randomSpawnTile = GridManager.Instance.GetPlayerBuildingSpawnTile();            

            randomSpawnTile.SetUnit(spawnedPlayerBuilding);
            _playerBuildingCount++;

            GameManager.Instance.ChangeState(GameState.SpawnEnemyBuilding);
        }
        else { 
            GameManager.Instance.ChangeState(GameState.SpawnPlayerRobot);
        }

    }

    public void SpawnEnemyBuilding()
    {
        Debug.Log("Enemy Building Spawn");
        if (_enemyBuildingCount < _buildingCount)
        {
            /*var randomPrefab = GetRandomUnit<BaseBuilding>(Faction.Enemy);
            var spawnedEnemyBuilding = Instantiate(randomPrefab);*/

            var spawnedEnemyBuilding = Instantiate(_enemyBuilding);
            spawnedEnemyBuilding.GetComponent<SpriteRenderer>().sprite = _enemyBuildingSprite;
            var randomSpawnTile = GridManager.Instance.GetEnemyBuildingSpawnTile();

            randomSpawnTile.SetUnit(spawnedEnemyBuilding);
            _enemyBuildingCount++;

            GameManager.Instance.ChangeState(GameState.SpawnPlayerBuilding);
        }
        else
        {
            GameManager.Instance.ChangeState(GameState.SpawnEnemyRobot);
        }
    }

    public void SpawnPlayerRobot()
    {
        Debug.Log("Player Spawn");
        if (!_playerSpawned)
        {
            /*var randomPrefab = GetRandomUnit<BaseRobot>(Faction.Player);
            var spawnedPlayerRobot = Instantiate(randomPrefab);*/

            var spawnedPlayerRobot = Instantiate(_playerRobot);
            spawnedPlayerRobot.GetComponent<SpriteRenderer>().sprite = _playerRobotSprite;
            var randomSpawnTile = GridManager.Instance.GetPlayerSpawnTile();
            randomSpawnTile.SetUnit(spawnedPlayerRobot);
            PlayerController.Instance.InitPlayer(spawnedPlayerRobot);
            _playerSpawned = true;

            GameManager.Instance.ChangeState(GameState.SpawnEnemyRobot);
        }
        else
        {
            GameManager.Instance.ChangeState(GameState.EnemyTurn);
        }

    }

    public void SpawnEnemyRobot()
    {
        Debug.Log("Enemy Spawn");
        if (!_enemySpawned)
        {
            /*var randomPrefab = GetRandomUnit<BaseRobot>(Faction.Enemy);
            var spawnedEnemyRobot = Instantiate(randomPrefab);*/

            var spawnedEnemyRobot = Instantiate(_enemyRobot);
            spawnedEnemyRobot.GetComponent<SpriteRenderer>().sprite = _enemyRobotSprite;
            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();

            randomSpawnTile.SetUnit(spawnedEnemyRobot);
            PlayerController.Instance.InitEnemy(spawnedEnemyRobot);
            _enemySpawned = true;

            GameManager.Instance.ChangeState(GameState.SpawnPlayerRobot);
        }
        else
        {
            GameManager.Instance.ChangeState(GameState.PlayerTurn);
        }
    }

    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        //return (T)_units.Where(u => u.Faction == faction && u.UnitPrefab is T).OrderBy(O => Random.value).First().UnitPrefab;
        return null;
    }
}
