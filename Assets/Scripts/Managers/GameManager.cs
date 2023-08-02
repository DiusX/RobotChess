using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{//based on Youtube tutorials from @Tarodev

    /// <summary>
    /// This Singleton class holds and manages the gamestate which is used to operate the flow of the game.
    /// </summary>
    /// TODO: Adjust class to be server code.
    public static GameManager Instance;
    public GameState Gamestate;

    void Awake(){
        Instance = this;
    }

    void Start(){
        ChangeState(GameState.GenerateGrid);
    }

    /// <summary>
    /// This method changes the gameState to the new called state.
    /// </summary>
    /// <param name="newState">The new state to change the gameState to</param>
    /// <exception cref="ArgumentOutOfRangeException">The given state does not fit in the game loop</exception>
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

/// <summary>
/// The enum values to use as input for changing the gameState in GameManager class.
/// </summary>
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