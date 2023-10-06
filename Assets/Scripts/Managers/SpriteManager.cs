using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    /// <summary>
    /// This Singleton class holds and manages all the information and logic required to set and change game sprites.
    /// </summary>
    /// TODO: Adjust class to retrieve info from server DB to match both player's settings.
    public static SpriteManager Instance;

    [SerializeField] private Sprite[] _unitSprites;
    [SerializeField] private Sprite[] _tokenSprites;
    [SerializeField] private Sprite[] _tileSprites;
    [SerializeField] private Sprite _stunSprite;
    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Gets the sprite associated with the robot facing south.
    /// </summary>
    /// <returns>The robot sprite</returns>
    public Sprite GetRobotSouthSprite() { 
        return _unitSprites[0];
    }

    /// <summary>
    /// Gets the sprite associated with the robot facing south.
    /// </summary>
    /// <returns>The robot sprite</returns>
    public Sprite GetRobotWestSprite()
    {
        return _unitSprites[1];
    }

    /// <summary>
    /// Gets the sprite associated with the robot facing south.
    /// </summary>
    /// <returns>The robot sprite</returns>
    public Sprite GetRobotNorthSprite()
    {
        return _unitSprites[2];
    }

    /// <summary>
    /// Gets the sprite associated with the robot facing south.
    /// </summary>
    /// <returns>The robot sprite</returns>
    public Sprite GetRobotEastSprite()
    {
        return _unitSprites[3];
    }


    /// <summary>
    /// Gets the sprite associated with the  buildings.
    /// </summary>
    /// <returns>The building sprite</returns>
    public Sprite GetBuildingSprite()
    {
        return _unitSprites[4];
    }

    /// <summary>
    /// Gets the sprite associated with the player's buildings when captured by the enemy.
    /// </summary> {Might be reworked to overlay player's flag on enemy building instead}
    /// <returns>The player's building sprite for when in captured state</returns> {Might be reworked to return flag sprite for player}
    public Sprite GetPlayerCaptureSprite()
    {
        return _unitSprites[5];
    }

    /// <summary>
    /// Gets the sprite associated with the enemy's buildings when captured by the player.
    /// </summary> {Might be reworked to overlay enemy's flag on player building instead}
    /// <returns>The enemy's building sprite for when in captured state</returns> {Might be reworked to return flag sprite for enemy}
    public Sprite GetEnemyCaptureSprite()
    {
        return _unitSprites[6];
    }

    /// <summary>
    /// Gets the sprite associated with astroid.
    /// </summary>
    /// <returns>The sprite for astroid</returns>
    public Sprite GetAstroidSprite()
    {
        return _unitSprites[7];
    }



    /// <summary>
    /// Gets the sprite associated with the 'Move Forward' token.
    /// </summary>
    /// <returns>'Move Forward' token's sprite</returns>
    public Sprite GetForwardTokenSprite()
    {
        return _tokenSprites[0];
    }

    /// <summary>
    /// Gets the sprite associated with the 'Move Backwards' token.
    /// </summary>
    /// <returns>'Move Backwards' token's sprite</returns>
    public Sprite GetBackwardsTokenSprite()
    {
        return _tokenSprites[1];
    }

    /// <summary>
    /// Gets the sprite associated with the 'Turn Left' token.
    /// </summary>
    /// <returns>'Turn Left' token's sprite</returns>
    public Sprite GetLeftTokenSprite()
    {
        return _tokenSprites[2];
    }

    /// <summary>
    /// Gets the sprite associated with the 'Turn Right' token.
    /// </summary>
    /// <returns>'Turn Right' token's sprite</returns>
    public Sprite GetRightTokenSprite()
    {
        return _tokenSprites[3];
    }

    /// <summary>
    /// Gets the sprite associated with the 'Capture' token.
    /// </summary>
    /// <returns>'Capture' token's sprite</returns>
    public Sprite GetCaptureTokenSprite()
    {
        return _tokenSprites[4];
    }

    /// <summary>
    /// Gets the sprite associated with the 'Shoot' token.
    /// </summary>
    /// <returns>'Shoot' token's sprite</returns>
    public Sprite GetShootTokenSprite()
    {
        return _tokenSprites[5];
    }

    /// <summary>
    /// Gets a random walkable Tile Sprite
    /// </summary>
    /// <returns>Tile Sprite</returns>
    public Sprite GetWalkableTileSprite()
    {
        return _tileSprites[Random.Range(0, 3)];
    }

    /// <summary>
    /// Gets a random unwalkable Tile Sprite
    /// </summary>
    /// <returns>Tile Sprite</returns>
    public Sprite GetUnwalkableTileSprite()
    {
        return _tileSprites[Random.Range(0, _tileSprites.Count())];
    }

    /// <summary>
    /// Gets a Stun Sprite to indicate less movement for a robot.
    /// </summary>
    /// <returns>Stun Sprite</returns>
    public Sprite GetStunSprite()
    {
        return _stunSprite;
    }
}
