using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{//From @Tarodev Youtube tutorials, has been adapted
    public string TileName;
   [SerializeField] protected SpriteRenderer _renderer;
   [SerializeField] private GameObject _highlight;
   [SerializeField] private GameObject _highlightPlaceable;
   [SerializeField] private bool _isWalkable;
    private bool _captured;
    private bool _ignoreUnit;

    public BaseUnit OccupiedUnit;
    public bool Walkable => _isWalkable && (OccupiedUnit == null || _ignoreUnit);
    public bool Captured => _captured;
    
    public void SetIgnoreUnit(bool ignoreUnit)
    {
        this._ignoreUnit = ignoreUnit;
    }

    public bool UnitIsIgnored() { 
        return _ignoreUnit; 
    }

    public virtual void Init(int x, int y) {
        _captured = false;
        _highlightPlaceable.GetComponent<SpriteRenderer>().enabled = false;
        //_highlightPlaceable.SetActive(false);
    }

    void OnMouseEnter(){
      _highlight.SetActive(true);
       TileManager.Instance.ShowTileInfo(this);
    }

    void OnMouseExit(){
       _highlight.SetActive(false);
        TileManager.Instance.ShowTileInfo(this);
    }

    public void SetHighlightPlaceable(bool value)
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
            //call placement unto manager
            TileManager.Instance.UnhighlightPlaceableTiles();
            switch (GameManager.Instance.Gamestate)
            {
                case (GameState.SpawnPlayerBuilding) : UnitManager.Instance.SpawnPlayerBuilding(this); break;
                case (GameState.SpawnEnemyBuilding) : UnitManager.Instance.SpawnEnemyBuilding(this); break;
                case (GameState.SpawnPlayerRobot) : UnitManager.Instance.SpawnPlayerRobot(this); break;
                case (GameState.SpawnEnemyRobot) : UnitManager.Instance.SpawnEnemyRobot(this); break;
            }            
        }
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
        if(!_captured && OccupiedUnit != null)
        {
            Sprite captureSprite = faction.Equals(Faction.Player) ? SpriteManager.Instance.GetPlayerCaptureSprite() : SpriteManager.Instance.GetEnemyCaptureSprite();
            OccupiedUnit.GetComponent<SpriteRenderer>().sprite = captureSprite;
            _captured = true;
        }
    }
}
