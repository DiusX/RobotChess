using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Tile : NetworkBehaviour
{//From @Tarodev Youtube tutorials, has been adapted
    public string TileName;
   [SerializeField] protected SpriteRenderer _renderer;
   [SerializeField] private GameObject _highlight;
   [SerializeField] private GameObject _highlightPlaceable;
   [SerializeField] private bool _isWalkable;
    private NetworkVariable<bool> _captured = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> _ignoreUnit = new NetworkVariable<bool>(false);

    public BaseUnit OccupiedUnit;
    public bool Walkable => _isWalkable && (OccupiedUnit == null || _ignoreUnit.Value);
    public bool Captured => _captured.Value;
    
    public void SetIgnoreUnit(bool ignoreUnit)
    {
        _ignoreUnit.Value = ignoreUnit;
    }

    public bool UnitIsIgnored() { 
        return _ignoreUnit.Value; 
    }

    public virtual void Init(int x, int y) {
        _highlightPlaceable.GetComponent<SpriteRenderer>().enabled = false;
        //_highlightPlaceable.SetActive(false);
    }

    [ClientRpc]
    public virtual void InitClientRpc(Vector2 vector)
    {
        Init((int)vector.x, (int)vector.y);
        name = "(" + transform.position.x + "," + transform.position.y + ") Tile";
    }

    void OnMouseEnter(){
      _highlight.SetActive(true);
       TileManager.Instance.ShowTileInfo(this);
    }

    void OnMouseExit(){
       _highlight.SetActive(false);
        TileManager.Instance.ShowTileInfo(this);
    }

    [ClientRpc]
    public void SetHighlightPlaceableClientRpc(bool value)
    {
        _highlightPlaceable.GetComponent<SpriteRenderer>().enabled = value;
        //_highlightPlaceable.SetActive(value);
    }

    private void OnMouseDown()
    {
        //Debug.Log(transform.position);

        /*if (GameManager.Instance.Gamestate != GameState.PlayerTurn) return;

        if (OccupiedUnit != null)
        {
            if (OccupiedUnit.Faction == Faction.Player) UnitManager.Instance.SetSelectedPlayer((BaseRobot)OccupiedUnit);
            else
            {
                if(UnitManager.Instance.selectedPlayer != null)
                {
                    var enemy = (BaseRobot)OccupiedUnit;
                    Destroy(enemy.gameObject);
                    SetUnit(UnitManager.Instance.selectedPlayer);
                    UnitManager.Instance.SetSelectedPlayer(null);
                }
            }
        }
        else
        {
            if (UnitManager.Instance.selectedPlayer != null)
            {
                SetUnit(UnitManager.Instance.selectedPlayer);
                UnitManager.Instance.SetSelectedPlayer(null);
            }
        }*/

        //if (this is GrassTile && _highlightPlaceable.activeSelf)
        if (this is GrassTile && _highlightPlaceable.GetComponent<SpriteRenderer>().enabled)
        {
            Debug.Log("Clicked on Tile: " + transform.position.x + ", " + transform.position.y);
            doPlacementServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void doPlacementServerRpc()
    {
        //TODO:  Implement Manager that checks player turn vs clientId and then limit input to that client only
        //
        //var clientId = serverRpcParams.Receive.SenderClientId;
        //if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        //{
        //    var client = NetworkManager.ConnectedClients[clientId];
        //    // Do things for this client
        //}


        //call placement unto manager
        TileManager.Instance.UnhighlightPlaceableTiles();
        UnitManager.Instance.SpawnUnit(this);
    }

    public void SetUnit(BaseUnit unit)
    {
        if (unit.OccupiedTile != null) unit.OccupiedTile.OccupiedUnit = null;
        unit.transform.position = transform.position;
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
    }
   
    public void CaptureBuilding(Faction faction)
    {
        if(!_captured.Value && OccupiedUnit != null)
        {
            Sprite captureSprite = faction.Equals(Faction.Player) ? SpriteManager.Instance.GetPlayerCaptureSprite() : SpriteManager.Instance.GetEnemyCaptureSprite();
            OccupiedUnit.GetComponent<SpriteRenderer>().sprite = captureSprite;
            _captured.Value = true;
        }
    }

    [ClientRpc]
    public void AddToPlayableListClientRpc()
    {
        TileManager.Instance.AddPlayableTile(this);
    }
}
