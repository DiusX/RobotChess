using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBuilding : BaseUnit
{
    private bool _isShielded;
    public bool IsShielded => _isShielded;
    public override void GetShot(Faction faction)
    {
        if(Faction == faction)
        {
            _isShielded = true;
        }
        else _isShielded = false;
    }

    public override void ClearShot()
    {
        _isShielded = false;
    }
}
