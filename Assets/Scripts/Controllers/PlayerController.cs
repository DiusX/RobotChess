using UnityEditor.Experimental.GraphView;
using UnityEditor.VersionControl;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    private BaseRobot playerRobot;
    private BaseRobot enemyRobot;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Obtains a reference to the player's robot
    /// </summary>
    /// <param name="player">The player robot</param>
    public void InitPlayer(BaseRobot player)
    {
        Debug.Log("Initing Player");
        playerRobot = player;
    }

    /// <summary>
    /// Obtains a reference to the enemy's robot
    /// </summary>
    /// <param name="enemy"></param>
    public void InitEnemy(BaseRobot enemy)
    {
        Debug.Log("Initing Enemy");
        enemyRobot = enemy;
    }

    /// <summary>
    /// Gets the Tile that the robot of a given faction is positioned on
    /// </summary>
    /// <param name="faction">The faction that the Robot belongs to: Player or Enemy</param>
    /// <returns>The tile that is occupied by the robot</returns>
    public Tile getRobotPosition(Faction faction)
    {
        return faction == Faction.Player ? playerRobot.OccupiedTile : enemyRobot.OccupiedTile;
    }

    /// <summary>
    /// Gets the direction that the robot of a given faction is facing.
    /// </summary>
    /// <param name="faction">The faction that the Robot belongs to: Player or Enemy</param>
    /// <returns>BaseRobot.Direction - the direction the robot is facing towards</returns>
    public BaseRobot.Direction getRobotDirection(Faction faction)
    {
        return faction == Faction.Player ? playerRobot.direction : enemyRobot.direction;
    }

    /// <summary>
    /// Call a method to move the player's robot one block forward in its direction
    /// </summary>
    public void MovePlayerForward()
    {
        Debug.Log("Player move Forward clicked");
        moveForward(playerRobot);
    }

    /// <summary>
    /// Call a method to move the player's robot one block backwards in its direction
    /// </summary>
    public void MovePlayerBackwards()
    {
        Debug.Log("Player move Backward clicked");
        moveBackwards(playerRobot);
    }

    /// <summary>
    /// Calls a method to turn the player robot 90deg left. <br />
    /// (Also currently rotates the sprite to indicate direction. TODO: Rework)
    /// </summary>
    public void TurnPlayerLeft()
    {
        Debug.Log("Player Turn Left clicked");
        turnLeft(playerRobot);
        playerRobot.transform.Rotate(0, 0, 90);
    }

    /// <summary>
    /// Calls a method to turn the player robot 90deg right. <br />
    /// (Also currently rotates the sprite to indicate direction. TODO: Rework)
    /// </summary>
    public void TurnPlayerRight()
    {
        Debug.Log("Player Turn Right clicked");
        turnRight(playerRobot);
        playerRobot.transform.Rotate(0, 0, -90);
    }

    /// <summary>
    /// Calls a method to let the player's robot attempt a capture
    /// </summary>
    public void PlayerCapture()
    {
        attemptCapture(playerRobot);
    }

    /// <summary>
    /// Call a method to move the enemy's robot one block forward in its direction
    /// </summary>
    public void MoveEnemyForward()
    {
        moveForward(enemyRobot);
    }

    /// <summary>
    /// Call a method to move the enemy's robot one block backwards in its direction
    /// </summary>
    public void MoveEnemyBackwards()
    {
        moveBackwards(enemyRobot);
    }

    /// <summary>
    /// Calls a method to turn the enemy robot 90deg left. <br />
    /// (Also currently rotates the sprite to indicate direction. TODO: Rework)
    /// </summary>
    public void TurnEnemyLeft()
    {
        turnLeft(enemyRobot);
        enemyRobot.transform.Rotate(0, 0, 90);
    }

    /// <summary>
    /// Calls a method to turn the enemy robot 90deg right. <br />
    /// (Also currently rotates the sprite to indicate direction. TODO: Rework)
    /// </summary>
    public void TurnEnemyRight()
    {
        turnRight(enemyRobot);
        enemyRobot.transform.Rotate(0, 0, -90);
    }

    /// <summary>
    /// Calls a method to let the enemy's robot attempt a capture
    /// </summary>
    public void EnemyCapture()
    {
        attemptCapture(enemyRobot);
    }

    public Tile GetForward(Tile tile, BaseRobot.Direction direction, out string message)
    {
        Tile tileForward;
        message = null;
        switch (direction)
        {
            case BaseRobot.Direction.South:
                tileForward = GridManager.Instance.GetTileSouthOfPosition(tile.transform.position);
                if (tileForward is not null && tileForward.Walkable)
                {
                    return tileForward;
                }
                message = "Tile forward to South is not Walkable";
                break;
            case BaseRobot.Direction.West:
                tileForward = GridManager.Instance.GetTileWestOfPosition(tile.transform.position);
                if (tileForward is not null && tileForward.Walkable)
                {
                    return tileForward;
                }
                message = "Tile forward to West is not Walkable";
                break;
            case BaseRobot.Direction.North:
                tileForward = GridManager.Instance.GetTileNorthOfPosition(tile.transform.position);
                if (tileForward is not null && tileForward.Walkable)
                {
                    return tileForward;
                }
                message = "Tile forward to North is not Walkable";
                break;
            case BaseRobot.Direction.East:
                tileForward = GridManager.Instance.GetTileEastOfPosition(tile.transform.position);
                if (tileForward is not null && tileForward.Walkable)
                {
                    return tileForward;
                }
                message = "Tile forward to East is not Walkable";
                break;
            default: return null;
        }
        return null;
    }

    public Tile GetBackwards(Tile tile, BaseRobot.Direction direction, out string message)
    {
        Tile tileBackwards;
        message = null;
        switch (direction)
        {
            case BaseRobot.Direction.South:
                tileBackwards = GridManager.Instance.GetTileNorthOfPosition(tile.transform.position);
                if (tileBackwards is not null && tileBackwards.Walkable)
                {
                    return tileBackwards;
                }
                message = "Tile backwards from South is not Walkable";
                break;
            case BaseRobot.Direction.West:
                tileBackwards = GridManager.Instance.GetTileEastOfPosition(tile.transform.position);
                if (tileBackwards is not null && tileBackwards.Walkable)
                {
                    return tileBackwards;
                }
                message = "Tile backwards from West is not Walkable";
                break;
            case BaseRobot.Direction.North:
                tileBackwards = GridManager.Instance.GetTileSouthOfPosition(tile.transform.position);
                if (tileBackwards is not null && tileBackwards.Walkable)
                {
                    return tileBackwards;
                }
                message = "Tile backwards from North is not Walkable";
                break;
            case BaseRobot.Direction.East:
                tileBackwards = GridManager.Instance.GetTileWestOfPosition(tile.transform.position);
                if (tileBackwards is not null && tileBackwards.Walkable)
                {
                    return tileBackwards;
                }
                message = "Tile backwards from East is not Walkable";
                break;
            default: return null;
        }
        return null;
    }

    public BaseRobot.Direction GetRightTurn(BaseRobot.Direction direction)
    {
        switch (direction)
        {
            case BaseRobot.Direction.North: return BaseRobot.Direction.East;
            case BaseRobot.Direction.East: return BaseRobot.Direction.South;
            case BaseRobot.Direction.South: return BaseRobot.Direction.West;
            case BaseRobot.Direction.West: return BaseRobot.Direction.North;
            default: return BaseRobot.Direction.South;
        }
    }

    public BaseRobot.Direction GetLeftTurn(BaseRobot.Direction direction)
    {
        switch (direction)
        {
            case BaseRobot.Direction.North: return BaseRobot.Direction.West;
            case BaseRobot.Direction.East: return BaseRobot.Direction.North;
            case BaseRobot.Direction.South: return BaseRobot.Direction.East;
            case BaseRobot.Direction.West: return BaseRobot.Direction.South;
            default: return BaseRobot.Direction.South;
        }
    }

    public bool GetCapture(Tile tile, BaseRobot.Direction direction, Faction faction, out string message)
    {
        Tile tileToCapture = null;
        message = null;
        switch (direction)
        {
            case BaseRobot.Direction.South:
                tileToCapture = GridManager.Instance.GetTileSouthOfPosition(tile.transform.position);
                break;
            case BaseRobot.Direction.West:
                tileToCapture = GridManager.Instance.GetTileWestOfPosition(tile.transform.position);
                break;
            case BaseRobot.Direction.North:
                tileToCapture = GridManager.Instance.GetTileNorthOfPosition(tile.transform.position);
                break;
            case BaseRobot.Direction.East:
                tileToCapture = GridManager.Instance.GetTileEastOfPosition(tile.transform.position);
                break;
            default: break;
        }
        if (tileToCapture is not null && tileToCapture.OccupiedUnit is not null)
        {
            if (tileToCapture.OccupiedUnit.Faction != faction)
            {
                if (!tileToCapture.Captured)
                {
                    return true;
                }
                else
                {
                    message = "The building has already been captured";
                }
            }
            else
            {
                if (!tileToCapture.unitIsIgnored())
                {
                    message = "You can not capture your own building";
                }
            }
        }
        else
        {
            message = "There is nothing to capture";
        }
        return false;
    }

    private void moveForward(BaseRobot robot) {
        Debug.Log("Attempting move from Tile: " + robot.transform.position.ToString());
        Tile tileForward;
        switch (robot.direction) {
            case BaseRobot.Direction.South:
                tileForward = GridManager.Instance.GetTileSouthOfPosition(robot.OccupiedTile.transform.position);
                if (tileForward is not null && tileForward.Walkable) {
                    tileForward.SetUnit(robot);
                    Debug.Log("Robot moving forward in South direction");
                    Debug.Log("Moved to Tile: " + tileForward.transform.position.ToString());
                }
                else if(tileForward is null)
                {
                    Debug.Log("Tile forward to South is null");
                }
                else if (!tileForward.Walkable)
                {
                    Debug.Log("Tile forward to South is not Walkable");
                }
                else
                {
                    Debug.Log("Error moving forward towards South tile");
                }
                break;
            case BaseRobot.Direction.West:
                tileForward = GridManager.Instance.GetTileWestOfPosition(robot.OccupiedTile.transform.position);
                if (tileForward is not null && tileForward.Walkable)
                {
                    tileForward.SetUnit(robot);
                    Debug.Log("Robot moving forward in West direction");
                    Debug.Log("Moved to Tile: " + tileForward.transform.position.ToString());
                }
                else if (tileForward is null)
                {
                    Debug.Log("Tile forward to West is null");
                }
                else if (!tileForward.Walkable)
                {
                    Debug.Log("Tile forward to West is not Walkable");
                }
                else
                {
                    Debug.Log("Error moving forward towards West tile");
                }
                break;
            case BaseRobot.Direction.North:
                tileForward = GridManager.Instance.GetTileNorthOfPosition(robot.OccupiedTile.transform.position);
                if (tileForward is not null && tileForward.Walkable)
                {
                    tileForward.SetUnit(robot);
                    Debug.Log("Robot moving forward in North direction");
                    Debug.Log("Moved to Tile: " + tileForward.transform.position.ToString());
                }
                else if (tileForward is null)
                {
                    Debug.Log("Tile forward to North is null");
                }
                else if (!tileForward.Walkable)
                {
                    Debug.Log("Tile forward to North is not Walkable");
                }
                else
                {
                    Debug.Log("Error moving forward towards North tile");
                }
                break;
            case BaseRobot.Direction.East:
                tileForward = GridManager.Instance.GetTileEastOfPosition(robot.OccupiedTile.transform.position);
                if (tileForward is not null && tileForward.Walkable)
                {
                    tileForward.SetUnit(robot);
                    Debug.Log("Robot moving forward in East direction");
                    Debug.Log("Moved to Tile: " + tileForward.transform.position.ToString());
                }
                else if (tileForward is null)
                {
                    Debug.Log("Tile forward to East is null");
                }
                else if (!tileForward.Walkable)
                {
                    Debug.Log("Tile forward to East is not Walkable");
                }
                else
                {
                    Debug.Log("Error moving forward towards East tile");
                }
                break;
            default: break;
        }
    }

    private void moveBackwards(BaseRobot robot)
    {
        Debug.Log("Attempting move from Tile: " + robot.transform.position.ToString());
        Tile tileBackwards;
        switch (robot.direction)
        {            
            case BaseRobot.Direction.South:
                tileBackwards = GridManager.Instance.GetTileNorthOfPosition(robot.OccupiedTile.transform.position);
                if (tileBackwards is not null && tileBackwards.Walkable)
                {                    
                    tileBackwards.SetUnit(robot);
                    Debug.Log("Robot moving backwards in North direction");
                    Debug.Log("Moved to Tile: " + tileBackwards.transform.position.ToString());
                }
                else if (tileBackwards is null)
                {
                    Debug.Log("Tile backwards from South is null");
                }
                else if (!tileBackwards.Walkable)
                {
                    Debug.Log("Tile backwards from South is not Walkable");
                }
                else
                {
                    Debug.Log("Error moving backwards towards North tile");
                }
                break;
            case BaseRobot.Direction.West:
                tileBackwards = GridManager.Instance.GetTileEastOfPosition(robot.OccupiedTile.transform.position);
                if (tileBackwards is not null && tileBackwards.Walkable)
                {
                    tileBackwards.SetUnit(robot);
                    Debug.Log("Robot moving backwards in East direction");
                    Debug.Log("Moved to Tile: " + tileBackwards.transform.position.ToString());
                }
                else if (tileBackwards is null)
                {
                    Debug.Log("Tile backwards from West is null");
                }
                else if (!tileBackwards.Walkable)
                {
                    Debug.Log("Tile backwards from West is not Walkable");
                }
                else
                {
                    Debug.Log("Error moving backwards towards East tile");
                }
                break;
            case BaseRobot.Direction.North:
                tileBackwards = GridManager.Instance.GetTileSouthOfPosition(robot.OccupiedTile.transform.position);
                if (tileBackwards is not null && tileBackwards.Walkable)
                {
                    tileBackwards.SetUnit(robot);
                    Debug.Log("Robot moving backwards in South direction");
                    Debug.Log("Moved to Tile: " + tileBackwards.transform.position.ToString());
                }
                else if (tileBackwards is null)
                {
                    Debug.Log("Tile backwards from North is null");
                }
                else if (!tileBackwards.Walkable)
                {
                    Debug.Log("Tile backwards from North is not Walkable");
                }
                else
                {
                    Debug.Log("Error moving backwards towards South tile");
                }
                break;
            case BaseRobot.Direction.East:
                tileBackwards = GridManager.Instance.GetTileWestOfPosition(robot.OccupiedTile.transform.position);
                if (tileBackwards is not null && tileBackwards.Walkable)
                {
                    tileBackwards.SetUnit(robot);
                    Debug.Log("Robot moving backwards in West direction");
                    Debug.Log("Moved to Tile: " + tileBackwards.transform.position.ToString());
                }
                else if (tileBackwards is null)
                {
                    Debug.Log("Tile backwards from East is null");
                }
                else if (!tileBackwards.Walkable)
                {
                    Debug.Log("Tile backwards from East is not Walkable");
                }
                else
                {
                    Debug.Log("Error moving backwards towards West tile");
                }
                break;
            default: break;
        }
    }

    private void turnRight(BaseRobot robot)
    {
        robot.direction = GetRightTurn(robot.direction);
        Debug.Log("Robot turned right to " + robot.direction);
    }

    private void turnLeft(BaseRobot robot)
    {
        robot.direction = GetLeftTurn(robot.direction);
        Debug.Log("Robot turned left to " + robot.direction);
    }

    private void attemptCapture(BaseRobot robot)
    {
        Debug.Log("Attempting to capture from Tile: " + robot.transform.position.ToString());
        Tile tileToCapture;
        switch (robot.direction)
        {
            case BaseRobot.Direction.South:
                tileToCapture = GridManager.Instance.GetTileSouthOfPosition(robot.OccupiedTile.transform.position);
                if (tileToCapture is not null && tileToCapture.OccupiedUnit is not null)
                {
                    if(tileToCapture.OccupiedUnit.Faction != robot.Faction)
                    {
                        if (!tileToCapture.Captured)
                        {
                            tileToCapture.CaptureBuilding(robot.Faction);
                            Debug.Log("Robot capturing in South direction");
                        }
                        else
                        {
                            Debug.Log("Piece already captured");
                        }                        
                    }
                    else
                    {
                        Debug.Log("Robot can't capture own piece");
                    }
                }     
                else
                {
                    Debug.Log("Robot could not capture in South direction");
                }
                break;
            case BaseRobot.Direction.West:
                tileToCapture = GridManager.Instance.GetTileWestOfPosition(robot.OccupiedTile.transform.position);
                if (tileToCapture is not null && tileToCapture.OccupiedUnit is not null)
                {
                    if (tileToCapture.OccupiedUnit.Faction != robot.Faction)
                    {
                        if (!tileToCapture.Captured)
                        {
                            tileToCapture.CaptureBuilding(robot.Faction);
                            Debug.Log("Robot capturing in West direction");
                        }
                        else
                        {
                            Debug.Log("Piece already captured");
                        }
                    }
                }
                else
                {
                    Debug.Log("Robot could not capture in West direction");
                }
                break;
            case BaseRobot.Direction.North:
                tileToCapture = GridManager.Instance.GetTileNorthOfPosition(robot.OccupiedTile.transform.position);
                if (tileToCapture is not null && tileToCapture.OccupiedUnit is not null)
                {
                    if (tileToCapture.OccupiedUnit.Faction != robot.Faction)
                    {
                        if (!tileToCapture.Captured)
                        {
                            tileToCapture.CaptureBuilding(robot.Faction);
                            Debug.Log("Robot capturing in North direction");
                        }
                        else
                        {
                            Debug.Log("Piece already captured");
                        }
                    }
                }
                else
                {
                    Debug.Log("Robot could not capture in North direction");
                }
                break;
            case BaseRobot.Direction.East:
                tileToCapture = GridManager.Instance.GetTileEastOfPosition(robot.OccupiedTile.transform.position);
                if (tileToCapture.OccupiedUnit.Faction != robot.Faction)
                {
                    if (!tileToCapture.Captured)
                    {
                        tileToCapture.CaptureBuilding(robot.Faction);
                        Debug.Log("Robot capturing in East direction");
                    }
                    else
                    {
                        Debug.Log("Piece already captured");
                    }
                }
                else
                {
                    Debug.Log("Robot could not capture in East direction");
                }
                break;
            default: break;
        }
    }
}
