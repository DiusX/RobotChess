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
        PlayerTurnManager.Instance.InitialisePlayerOnServer(rpcParams.Receive.SenderClientId);
        if (allClientsReady)
        {
            RobotChessLobby.Instance.DeleteLobby();
            PlayerTurnManager.Instance.InitialisePlayersOnClients();
            PreGameUI.Instance.GameStartedClientRpc();
            ChangeStateServerRpc(GameState.GenerateGrid);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerUnReadyServerRpc(ServerRpcParams rpcParams = default)
    {
        Debug.Log("CALLING SetPlayerUnReadyServerRpc()");
        PlayerTurnManager.Instance.UnsetPlayerOnServer(rpcParams.Receive.SenderClientId);
        playerReadyDictionary[rpcParams.Receive.SenderClientId] = false;
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

            case GameState.SpawnPlayerBuilding:
                IEnumerable<KeyValuePair<Vector2, Tile>> _placeableTiles = GridManager.Instance.GetPlayerBuildingSpawnTiles();
                if (_placeableTiles.Count() > 0)
                {
                    TileManager.Instance.HighlightPlaceableTiles(_placeableTiles, Faction.Player);
                }
                else
                {
                    ChangeStateServerRpc(GameState.EnemyTurn);
                }
                break;

            case GameState.SpawnEnemyBuilding:
                _placeableTiles = GridManager.Instance.GetEnemyBuildingSpawnTiles();
                if (_placeableTiles.Count() > 0)
                {
                    TileManager.Instance.HighlightPlaceableTiles(_placeableTiles, Faction.Enemy);
                }
                else
                {
                    ChangeStateServerRpc(GameState.PlayerTurn);
                }
                break;

            case GameState.SpawnPlayerRobot:
                TileManager.Instance.HighlightPlaceableTiles(GridManager.Instance.GetPlayerSpawnTiles(), Faction.Player);
                break;

            case GameState.SpawnEnemyRobot:
                TileManager.Instance.HighlightPlaceableTiles(GridManager.Instance.GetEnemySpawnTiles(), Faction.Enemy);
                break;

            case GameState.PlayerTurn:
                UnitManager.Instance.ClearShotsOnTurnStart(Faction.Player);
                ScoreboardManager.Instance.UpdateScoreBoardClientRpc(UnitManager.Instance.ReturnPlayerScore(), UnitManager.Instance.ReturnEnemyScore());
                RobotController.Instance.UnlockServerRpc();

                Vector2 position = RobotController.Instance.GetRobotPositionForServer(Faction.Player); 
                UnitDirection direction = RobotController.Instance.GetRobotDirectionForServer(Faction.Player);
                bool stun = RobotController.Instance.isStunnedRobot(Faction.Player);
                bool ammo = RobotController.Instance.HasAmmo(Faction.Player);                
                InputController.Instance.InitTempRobotClientRpc(position, direction, stun, ammo, PlayerTurnManager.Instance.GetPlayerRpcParams(Faction.Player));
                
                CountdownTimerController.Instance.StartTimer();
                break;

            case GameState.EnemyTurn:
                UnitManager.Instance.ClearShotsOnTurnStart(Faction.Enemy);
                ScoreboardManager.Instance.UpdateScoreBoardClientRpc(UnitManager.Instance.ReturnPlayerScore(), UnitManager.Instance.ReturnEnemyScore());
                RobotController.Instance.UnlockServerRpc();

                position = RobotController.Instance.GetRobotPositionForServer(Faction.Enemy); 
                direction = RobotController.Instance.GetRobotDirectionForServer(Faction.Enemy);
                stun = RobotController.Instance.isStunnedRobot(Faction.Enemy);
                ammo = RobotController.Instance.HasAmmo(Faction.Enemy);
                InputController.Instance.InitTempRobotClientRpc(position, direction, stun, ammo, PlayerTurnManager.Instance.GetPlayerRpcParams(Faction.Enemy));
                
                CountdownTimerController.Instance.StartTimer();
                break;

            case GameState.GameOver:
                ScoreboardManager.Instance.DisableScoreBoardClientRpc();
                int scorePlayerOne = UnitManager.Instance.ReturnPlayerScore();
                int scorePlayerTwo = UnitManager.Instance.ReturnEnemyScore();
                Faction victorFaction = (scorePlayerOne > scorePlayerTwo) ? Faction.Player : Faction.Enemy;
                GameOverController.Instance.ShowEndScreenClientRpc(scorePlayerOne, scorePlayerTwo, victorFaction);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }
}

