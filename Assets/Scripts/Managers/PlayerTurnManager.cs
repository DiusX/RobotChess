using Unity.Netcode;
using UnityEngine;

public class PlayerTurnManager : NetworkBehaviour
{
    public static PlayerTurnManager Instance;

    private NetworkVariable<ulong> playerOneId = new NetworkVariable<ulong>();
    private NetworkVariable<ulong> playerTwoId = new NetworkVariable<ulong>();
    
    private string playerOneName;
    private string playerTwoName;

    private bool playerOneSet = false;
    private bool playerTwoSet = false;

    private void Awake()
    {
        Instance = this;
    }

    public void InitialisePlayerOnServer(ulong id)
    {
        if (!playerOneSet)
        {
            playerOneId.Value = id;
            playerOneSet = true;
        }
        else if (!playerTwoSet && id != playerOneId.Value)
        {
            playerTwoId.Value = id;
            playerTwoSet = true;
        }
    }

    public void UnsetPlayerOnServer(ulong id)
    {
        if (playerOneSet && playerOneId.Value == id) {
            playerOneSet = false;
        }
        else if (playerTwoSet && playerTwoId.Value == id)
        {
            playerTwoSet = false;
        }
    }

    public void InitialisePlayersOnClients()
    {
        Debug.LogWarning("Player One Id: " + playerOneId.Value);
        Debug.LogWarning("Player Two Id: " + playerTwoId.Value);
        InitialisePlayersClientRpc(playerOneId.Value, playerTwoId.Value, playerOneName, playerTwoName);
    }

    [ClientRpc]
    public void InitialisePlayersClientRpc(ulong playerOneIdValue, ulong playerTwoIdValue, string playerOneName, string playerTwoName)
    {
        playerOneId.Value = playerOneIdValue;
        playerTwoId.Value = playerTwoIdValue;
        this.playerOneName = playerOneName;
        this.playerTwoName = playerTwoName;
       
    }

    public bool IsPlayerTurn(ulong id) { 
        if(id == playerOneId.Value && GameManager.Instance.Gamestate.Value == GameState.PlayerTurn)
        {
            return true;
        }
        if (id == playerTwoId.Value && GameManager.Instance.Gamestate.Value == GameState.EnemyTurn)
        {
            return true;
        }
        return false;
    }

    public Faction GetPlayerFaction(ulong id)
    {
        Debug.LogWarning("CHECK HERE XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
        Debug.Log("The ID of OWNER is: " + id);
        if(id == playerOneId.Value)
        {
            return Faction.Player;
        }
        if(id == playerTwoId.Value)
        {
            return Faction.Enemy;
        }
        return Faction.Neutral;
    }

    public ClientRpcParams GetPlayerRpcParams(Faction faction)
    {
        if(faction == Faction.Player)
        {
            return new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { playerOneId.Value }
                }
            };
        }
        else
        {
            return new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { playerTwoId.Value }
                }
            };
        }
    }
}
