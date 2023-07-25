using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager Instance;

    [SerializeField] private Sprite[] UnitSprites;
    [SerializeField] private Sprite[] TokenSprites;
    void Awake()
    {
        Instance = this;
    }

    public Sprite GetPlayerRobotSprite() { 
        return UnitSprites[0];
    }
    public Sprite GetEnemyRobotSprite()
    {
        return UnitSprites[1];
    }
    public Sprite GetPlayerBuildingSprite()
    {
        return UnitSprites[2];
    }
    public Sprite GetEnemyBuildingSprite()
    {
        return UnitSprites[3];
    }
    public Sprite GetPlayerCaptureSprite()
    {
        return UnitSprites[4];
    }
    public Sprite GetEnemyCaptureSprite()
    {
        return UnitSprites[5];
    }

    public Sprite GetForwardTokenSprite()
    {
        return TokenSprites[0];
    }
    public Sprite GetBackwardsTokenSprite()
    {
        return TokenSprites[1];
    }
    public Sprite GetLeftTokenSprite()
    {
        return TokenSprites[2];
    }
    public Sprite GetRightTokenSprite()
    {
        return TokenSprites[3];
    }    
    public Sprite GetCaptureTokenSprite()
    {
        return TokenSprites[4];
    }
}
