using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The enum values to use as input for changing the gameState in GameManager class.
/// </summary>
public enum GameState
{
    StartUp = -1,
    GenerateGrid = 0,
    SpawnPlayerBuilding = 1,
    SpawnEnemyBuilding = 2,
    SpawnPlayerRobot = 3,
    SpawnEnemyRobot = 4,
    PlayerTurn = 5,
    EnemyTurn = 6,
    GameOver = 7
}
