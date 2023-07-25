using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState Gamestate;

    void Awake(){
        Instance = this;
    }

    void Start(){
        ChangeState(GameState.GenerateGrid);
    }

    public void ChangeState(GameState newState){
        Gamestate = newState;
        switch (newState)
        {
            case GameState.GenerateGrid:
                GridManager.Instance.GenerateGrid();    
                break;
            case GameState.SpawnPlayerBuilding:
                UnitManager.Instance.SpawnPlayerBuilding();
                break;
            case GameState.SpawnEnemyBuilding:
                UnitManager.Instance.SpawnEnemyBuilding();
                break;
            case GameState.SpawnPlayerRobot:
                UnitManager.Instance.SpawnPlayerRobot();
                break;
            case GameState.SpawnEnemyRobot:
                UnitManager.Instance.SpawnEnemyRobot();
                break;
            case GameState.PlayerTurn:
                InputController.Instance.InitTempRobot(PlayerController.Instance.getRobotPosition(Faction.Player), PlayerController.Instance.getRobotDirection(Faction.Player));
                break;
            case GameState.EnemyTurn:
                InputController.Instance.InitTempRobot(PlayerController.Instance.getRobotPosition(Faction.Enemy), PlayerController.Instance.getRobotDirection(Faction.Enemy));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }
}

public enum GameState
{
    GenerateGrid = 0,
    SpawnPlayerBuilding = 1,
    SpawnEnemyBuilding = 2,
    SpawnPlayerRobot = 3,
    SpawnEnemyRobot = 4,
    PlayerTurn = 5,
    EnemyTurn = 6

}