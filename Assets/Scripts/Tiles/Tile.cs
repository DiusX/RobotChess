using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UI;

public abstract class Tile : MonoBehaviour
{//From @Tarodev Youtube tutorials, has been adapted
    public string TileName;
   [SerializeField] protected SpriteRenderer _renderer;
   [SerializeField] private GameObject _highlight;
   [SerializeField] private bool _isWalkable;
    
    protected bool _captured;
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
        
    }

    void OnMouseEnter(){
      _highlight.SetActive(true);
       TileManager.Instance.ShowTileInfo(this);
    }

    void OnMouseExit(){
       _highlight.SetActive(false);
        TileManager.Instance.ShowTileInfo(this);
    }    
}
