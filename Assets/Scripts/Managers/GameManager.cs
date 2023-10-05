using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    /// <summary>
    /// This Singleton class holds and manages the gamestate which is used to operate the flow of the game.
    /// </summary>
    public static GameManager Instance;
    public NetworkVariable<GameState> Gamestate = new NetworkVariable<GameState>(GameState.StartUp);
    private Dictionary<ulong, bool> playerReadyDictionary;
    
    void Awake(){
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    void Start(){
        Debug.Log("STARTING GAMEMANAGER Start()");
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerReadyServerRpc(ServerRpcParams rpcParams = default)
    {
        Debug.Log("CALLING SetPlayerReadyServerRpc()");
        playerReadyDictionary[rpcParams.Receive.SenderClientId] = true;
        bool allClientsReady = true;
        if(NetworkManager.Singleton.ConnectedClients.Count <= 1)
        {
            Debug.Log("Not enough players connected yet");
            return;
        }
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])  {
                // Found a player that's not ready
                allClientsReady = false;
                break;
            }

        }
        Debug.Log("All clients ready: " + allClientsReady);
        if (allClientsReady)
        {
            NetworkManagerUI.Instance.GameStartedClientRpc();
            ChangeStateServerRpc(GameState.GenerateGrid);
        }
    }

    /// <summary>
    /// This method changes the gameState to the new called state.
    /// </summary>
    /// <param name="newState">The new state to change the gameState to</param>
    /// <exception cref="ArgumentOutOfRangeException">The given state does not fit in the game loop</exception>
    [ServerRpc]
    public void ChangeStateServerRpc(GameState newState){
        Debug.Log("CHANGING GAMESTATE TO " + newState);
        Gamestate.Value = newState;
        Gamestate.SetDirty(true);
        switch (newState)
        {
            case GameState.GenerateGrid:
                GridManager.Instance.GenerateGridServerRpc(); 
                break;
            case GameState.SpawnPlayerBuilding: //TODO: limit to appropriate client
                {
                    IEnumerable<KeyValuePair<Vector2, Tile>> _placeableTiles = GridManager.Instance.GetPlayerBuildingSpawnTiles();
                    if(_placeableTiles.Count() > 0)
                    {
                        TileManager.Instance.HighlightPlaceableTiles(_placeableTiles);
                    }
                    else
                    {
                        ChangeStateServerRpc(GameState.EnemyTurn);
                    }
                    break;
                }                                
            case GameState.SpawnEnemyBuilding:
                {
                    IEnumerable<KeyValuePair<Vector2, Tile>> _placeableTiles = GridManager.Instance.GetEnemyBuildingSpawnTiles();
                    if (_placeableTiles.Count() > 0)
                    {
                        TileManager.Instance.HighlightPlaceableTiles(_placeableTiles);
                    }
                    else
                    {
                        ChangeStateServerRpc(GameState.PlayerTurn);
                    }
                    break;
                }
            case GameState.SpawnPlayerRobot:
                TileManager.Instance.HighlightPlaceableTiles(GridManager.Instance.GetPlayerSpawnTiles());
                break;
            case GameState.SpawnEnemyRobot:
                TileManager.Instance.HighlightPlaceableTiles(GridManager.Instance.GetEnemySpawnTiles());
                break;
            case GameState.PlayerTurn:
                UnitManager.Instance.ClearShotsOnTurnStart(Faction.Player);
                Vector2 position = RobotController.Instance.GetRobotPositionForServer(Faction.Player); 
                UnitDirection direction = RobotController.Instance.GetRobotDirectionForServer(Faction.Player); 
                Faction faction = Faction.Player; 
                bool stun = RobotController.Instance.isStunnedRobot(Faction.Player);
                bool ammo = RobotController.Instance.HasAmmo(Faction.Player);
                Debug.Log("Init variables: " + position + " ;  " + direction + " ;  " + faction + " ;  " + stun);
                InputController.Instance.InitTempRobotClientRpc(position, direction, faction, stun, ammo);
                //CountdownTimerController.Instance.StartTimer();
                break;
            case GameState.EnemyTurn:
                UnitManager.Instance.ClearShotsOnTurnStart(Faction.Enemy);
                position = RobotController.Instance.GetRobotPositionForServer(Faction.Enemy); 
                direction = RobotController.Instance.GetRobotDirectionForServer(Faction.Enemy); 
                faction = Faction.Enemy; 
                stun = RobotController.Instance.isStunnedRobot(Faction.Enemy);
                ammo = RobotController.Instance.HasAmmo(Faction.Enemy);
                Debug.Log("Init variables: " + position + " ;  " + direction + " ;  " + faction + " ;  " + stun);
                InputController.Instance.InitTempRobotClientRpc(position, direction, faction, stun, ammo);
                //CountdownTimerController.Instance.StartTimer();
                break;
            case GameState.GameOver:
                GameOverController.Instance.ShowEndScreenClientRpc();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }
}

