using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    /// <summary>
    /// This Singleton class holds and manages all the information and logic required to set and change game sprites.
    /// </summary>
    /// TODO: Adjust class to retrieve info from server DB to match both player's settings.
    public static SpriteManager Instance;

    [SerializeField] private Sprite[] UnitSprites;
    [SerializeField] private Sprite[] TokenSprites;
    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Gets the sprite associated with the robot looking South Direction.
    /// </summary>
    /// <returns>The robot sprite</returns>
    public Sprite GetRobotSouthSprite() { 
        return UnitSprites[0];
    }

    /// <summary>
    /// Gets the sprite associated with the robot looking West Direction.
    /// </summary>
    /// <returns>The robot sprite</returns>
    public Sprite GetRobotWestSprite()
    {
        return UnitSprites[1];
    }

    /// <summary>
    /// Gets the sprite associated with the robot looking North Direction.
    /// </summary>
    /// <returns>The robot sprite</returns>
    public Sprite GetRobotNorthSprite()
    {
        return UnitSprites[2];
    }

    /// <summary>
    /// Gets the sprite associated with the robot looking East Direction.
    /// </summary>
    /// <returns>The robot sprite</returns>
    public Sprite GetRobotEastSprite()
    {
        return UnitSprites[3];
    }

    /// <summary>
    /// Gets the sprite associated with the player's buildings.
    /// </summary>
    /// <returns>The player building sprite</returns>
    public Sprite GetPlayerBuildingSprite()
    {
        return UnitSprites[4];
    }

    /// <summary>
    /// Gets the sprite associated with the enemy's buildings.
    /// </summary>
    /// <returns>The enemy building sprite</returns>
    public Sprite GetEnemyBuildingSprite()
    {
        return UnitSprites[5];
    }

    /// <summary>
    /// Gets the sprite associated with the player's buildings when captured by the enemy.
    /// </summary> {Might be reworked to overlay player's flag on enemy building instead}
    /// <returns>The player's building sprite for when in captured state</returns> {Might be reworked to return flag sprite for player}
    public Sprite GetPlayerCaptureSprite()
    {
        return UnitSprites[6];
    }

    /// <summary>
    /// Gets the sprite associated with the enemy's buildings when captured by the player.
    /// </summary> {Might be reworked to overlay enemy's flag on player building instead}
    /// <returns>The enemy's building sprite for when in captured state</returns> {Might be reworked to return flag sprite for enemy}
    public Sprite GetEnemyCaptureSprite()
    {
        return UnitSprites[7];
    }



    /// <summary>
    /// Gets the sprite associated with the 'Move Forward' token.
    /// </summary>
    /// <returns>'Move Forward' token's sprite</returns>
    public Sprite GetForwardTokenSprite()
    {
        return TokenSprites[0];
    }

    /// <summary>
    /// Gets the sprite associated with the 'Move Backwards' token.
    /// </summary>
    /// <returns>'Move Backwards' token's sprite</returns>
    public Sprite GetBackwardsTokenSprite()
    {
        return TokenSprites[1];
    }

    /// <summary>
    /// Gets the sprite associated with the 'Turn Left' token.
    /// </summary>
    /// <returns>'Turn Left' token's sprite</returns>
    public Sprite GetLeftTokenSprite()
    {
        return TokenSprites[2];
    }

    /// <summary>
    /// Gets the sprite associated with the 'Turn Right' token.
    /// </summary>
    /// <returns>'Turn Right' token's sprite</returns>
    public Sprite GetRightTokenSprite()
    {
        return TokenSprites[3];
    }

    /// <summary>
    /// Gets the sprite associated with the 'Capture' token.
    /// </summary>
    /// <returns>'Capture' token's sprite</returns>
    public Sprite GetCaptureTokenSprite()
    {
        return TokenSprites[4];
    }

    /// <summary>
    /// Gets the sprite associated with the 'Shoot' token.
    /// </summary>
    /// <returns>'Shoot' token's sprite</returns>
    public Sprite GetShootTokenSprite()
    {
        return TokenSprites[5];
    }
}
