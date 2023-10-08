using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AnimationManager : NetworkBehaviour
{
    public static AnimationManager Instance;

    [SerializeField] private ParticleSystem _particleEffect;
    void Awake()
    {
        Instance = this;
    }
    public void UpdateRobotAnimation (BaseRobot robot)
    {
        switch(robot.direction.Value)
        {
            case UnitDirection.South: robot.ChangeAnimationState(BaseRobot.IDLE_SOUTH); robot.FlipSpriteHorizontally(false); break;
            case UnitDirection.West: robot.ChangeAnimationState(BaseRobot.IDLE_WEST); robot.FlipSpriteHorizontally(true); break;
            case UnitDirection.North: robot.ChangeAnimationState(BaseRobot.IDLE_NORTH); robot.FlipSpriteHorizontally(false); break;
            case UnitDirection.East: robot.ChangeAnimationState(BaseRobot.IDLE_EAST); robot.FlipSpriteHorizontally(false); break;
        }
    }

    [ClientRpc]
    public void PlayParticleClientRpc(Vector3 position)
    {
        _particleEffect.transform.position = position;
        _particleEffect.Play();
    }
}
