using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BaseRobot : BaseUnit
{
    public Direction direction;
    public enum Direction
    {
        South = 0,
        West = 1,
        North = 2,
        East = 3
    }    
}
