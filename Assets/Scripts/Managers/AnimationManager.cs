using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;
    void Awake()
    {
        Instance = this;
    }
    public void UpdateRobotAnimation (BaseRobot robot)
    {
        switch(robot.direction.Value)
        {
            case UnitDirection.South: robot.ChangeAnimationState(BaseRobot.IDLE_SOUTH); break;
            case UnitDirection.West: robot.ChangeAnimationState(BaseRobot.IDLE_WEST); break;
            case UnitDirection.North: robot.ChangeAnimationState(BaseRobot.IDLE_NORTH); break;
            case UnitDirection.East: robot.ChangeAnimationState(BaseRobot.IDLE_EAST); break;
        }
    }
}
