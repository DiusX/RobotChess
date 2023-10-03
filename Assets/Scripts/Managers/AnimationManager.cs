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
    public void UpdateRobotAnimation (BaseRobot robot, bool walking)
    {
        switch(robot.direction.Value)
        {
            case UnitDirection.South: robot.ChangeAnimationState(BaseRobot.IDLE_SOUTH); robot.FlipSpriteHorizontally(false); break;
            case UnitDirection.West: robot.ChangeAnimationState(BaseRobot.IDLE_WEST); robot.FlipSpriteHorizontally(true); break;
            case UnitDirection.North: robot.ChangeAnimationState(BaseRobot.IDLE_NORTH); robot.FlipSpriteHorizontally(false); break;
            case UnitDirection.East: robot.ChangeAnimationState(BaseRobot.IDLE_EAST); robot.FlipSpriteHorizontally(false); break;
        }
    }
}
