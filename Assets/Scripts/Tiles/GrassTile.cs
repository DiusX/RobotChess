using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class GrassTile : Tile
{ //From @Tarodev Youtube tutorials
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private GameObject _highlightPlaceable;

    public override void Init(int x, int y) {
        var isOffset = (x + y) % 2 == 1;
        _renderer.color = isOffset ? _offsetColor : _baseColor;
        _captured = false;
    }

    public void SetHighlightPlaceable(bool value)
    {
        _highlightPlaceable.SetActive(value);
    }

    private void OnMouseDown()
    {
        Debug.Log(transform.position);
        TileManager.Instance.UnhighlightPlaceableTiles();
        switch (GameManager.Instance.Gamestate)
        {
            case (GameState.SpawnPlayerBuilding): UnitManager.Instance.SpawnPlayerBuilding(this); break;
            case (GameState.SpawnEnemyBuilding): UnitManager.Instance.SpawnEnemyBuilding(this); break;
            case (GameState.SpawnPlayerRobot): UnitManager.Instance.SpawnPlayerRobot(this); break;
            case (GameState.SpawnEnemyRobot): UnitManager.Instance.SpawnEnemyRobot(this); break;
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
        if (!_captured && OccupiedUnit != null)
        {
            Sprite captureSprite = faction.Equals(Faction.Player) ? SpriteManager.Instance.GetPlayerCaptureSprite() : SpriteManager.Instance.GetEnemyCaptureSprite();
            OccupiedUnit.GetComponent<SpriteRenderer>().sprite = captureSprite;
            _captured = true;
        }
    }
}
