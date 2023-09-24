using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// This Singleton class handles all movement that occurs for the robots.
    /// </summary>
    [SerializeField] private int _startingAmmoCount;
    public static PlayerController Instance;
    private BaseRobot _playerRobot;
    private int _playerAmmo; //still need to indicate in UI
    private BaseRobot _enemyRobot;
    private int _enemyAmmo;

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
        _playerRobot = player;
        _playerAmmo = _startingAmmoCount;
    }

    /// <summary>
    /// Obtains a reference to the enemy's robot
    /// </summary>
    /// <param name="enemy"></param>
    public void InitEnemy(BaseRobot enemy)
    {
        Debug.Log("Initing Enemy");
        _enemyRobot = enemy;
        _enemyAmmo = _startingAmmoCount;
    }

    /// <summary>
    /// Gets the Tile that the robot of a given faction is positioned on
    /// </summary>
    /// <param name="faction">The faction that the Robot belongs to: Player or Enemy</param>
    /// <returns>The tile that is occupied by the robot</returns>
    public Tile getRobotPosition(Faction faction)
    {
        return faction == Faction.Player ? _playerRobot.OccupiedTile : _enemyRobot.OccupiedTile;
    }

    /// <summary>
    /// Gets the direction that the robot of a given faction is facing.
    /// </summary>
    /// <param name="faction">The faction that the Robot belongs to: Player or Enemy</param>
    /// <returns>BaseRobot.Direction - the direction the robot is facing towards</returns>
    public UnitDirection GetRobotDirection(Faction faction)
    {
        return faction == Faction.Player ? _playerRobot.direction : _enemyRobot.direction;
    }

    /// <summary>
    /// Call a method to move the player's robot one block forward in its direction
    /// </summary>
    public void MovePlayerForward()
    {
        Debug.Log("Player move Forward clicked");
        moveForward(_playerRobot);
    }

    /// <summary>
    /// Call a method to move the player's robot one block backwards in its direction
    /// </summary>
    public void MovePlayerBackwards()
    {
        Debug.Log("Player move Backward clicked");
        moveBackwards(_playerRobot);
    }

    /// <summary>
    /// Calls a method to turn the player robot 90deg left. <br />
    /// (Also currently rotates the sprite to indicate direction. TODO: Rework)
    /// </summary>
    public void TurnPlayerLeft()
    {
        Debug.Log("Player Turn Left clicked");
        turnLeft(_playerRobot);
        _playerRobot.transform.Rotate(0, 0, 90);
    }

    /// <summary>
    /// Calls a method to turn the player robot 90deg right. <br />
    /// (Also currently rotates the sprite to indicate direction. TODO: Rework)
    /// </summary>
    public void TurnPlayerRight()
    {
        Debug.Log("Player Turn Right clicked");
        turnRight(_playerRobot);
        _playerRobot.transform.Rotate(0, 0, -90);
    }

    /// <summary>
    /// Calls a method to let the player's robot attempt a capture
    /// </summary>
    public void PlayerCapture()
    {
        attemptCapture(_playerRobot);
    }

    /// <summary>
    /// Call a method to move the enemy's robot one block forward in its direction
    /// </summary>
    public void MoveEnemyForward()
    {
        moveForward(_enemyRobot);
    }

    /// <summary>
    /// Call a method to move the enemy's robot one block backwards in its direction
    /// </summary>
    public void MoveEnemyBackwards()
    {
        moveBackwards(_enemyRobot);
    }

    /// <summary>
    /// Calls a method to turn the enemy robot 90deg left. <br />
    /// (Also currently rotates the sprite to indicate direction. TODO: Rework)
    /// </summary>
    public void TurnEnemyLeft()
    {
        turnLeft(_enemyRobot);
        _enemyRobot.transform.Rotate(0, 0, 90);
    }

    /// <summary>
    /// Calls a method to turn the enemy robot 90deg right. <br />
    /// (Also currently rotates the sprite to indicate direction. TODO: Rework)
    /// </summary>
    public void TurnEnemyRight()
    {
        turnRight(_enemyRobot);
        _enemyRobot.transform.Rotate(0, 0, -90);
    }

    /// <summary>
    /// Calls a method to let the enemy's robot attempt a capture
    /// </summary>
    public void EnemyCapture()
    {
        attemptCapture(_enemyRobot);
    }

    /// <summary>
    /// Given a tile and a direction to check movement from, check if the tile infront of it will be walkable and return it. <br />
    /// </summary>
    /// <param name="tile">The tile from which to check.</param>
    /// <param name="direction">The direction in which the check should take place.</param>
    /// <param name="message">The feedback message that the player will get in the info popup if the tile in front is not walkable.</param>
    /// <returns>The tile in front of the given tile if it is walkable, else returns null</returns>
    public Tile GetForward(Tile tile, UnitDirection direction, out string message)
    {
        Tile tileForward;
        message = null;
        switch (direction)
        {
            case UnitDirection.South:
                tileForward = GridManager.Instance.GetTileSouthOfPosition(tile.transform.position);
                if (tileForward is not null && tileForward.Walkable)
                {
                    return tileForward;
                }
                message = "Tile forward to South is not Walkable";
                break;
            case UnitDirection.West:
                tileForward = GridManager.Instance.GetTileWestOfPosition(tile.transform.position);
                if (tileForward is not null && tileForward.Walkable)
                {
                    return tileForward;
                }
                message = "Tile forward to West is not Walkable";
                break;
            case UnitDirection.North:
                tileForward = GridManager.Instance.GetTileNorthOfPosition(tile.transform.position);
                if (tileForward is not null && tileForward.Walkable)
                {
                    return tileForward;
                }
                message = "Tile forward to North is not Walkable";
                break;
            case UnitDirection.East:
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

    /// <summary>
    /// Given a tile and a direction to check movement from, check if the tile behind it will be walkable and return it. <br />
    /// </summary>
    /// <param name="tile">The tile from which to check.</param>
    /// <param name="direction">The direction in which the check should take place.</param>
    /// <param name="message">The feedback message that the player will get in the info popup if the tile behind is not walkable.</param>
    /// <returns>The tile behind the given tile if it is walkable, else returns null</returns>
    public Tile GetBackwards(Tile tile, UnitDirection direction, out string message)
    {
        Tile tileBackwards;
        message = null;
        switch (direction)
        {
            case UnitDirection.South:
                tileBackwards = GridManager.Instance.GetTileNorthOfPosition(tile.transform.position);
                if (tileBackwards is not null && tileBackwards.Walkable)
                {
                    return tileBackwards;
                }
                message = "Tile backwards from South is not Walkable";
                break;
            case UnitDirection.West:
                tileBackwards = GridManager.Instance.GetTileEastOfPosition(tile.transform.position);
                if (tileBackwards is not null && tileBackwards.Walkable)
                {
                    return tileBackwards;
                }
                message = "Tile backwards from West is not Walkable";
                break;
            case UnitDirection.North:
                tileBackwards = GridManager.Instance.GetTileSouthOfPosition(tile.transform.position);
                if (tileBackwards is not null && tileBackwards.Walkable)
                {
                    return tileBackwards;
                }
                message = "Tile backwards from North is not Walkable";
                break;
            case UnitDirection.East:
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

    /// <summary>
    /// Given a direction, gets the direction turned 90deg clockwise.
    /// </summary>
    /// <param name="direction">The direction to turn right from.</param>
    /// <returns>The right-turned direction.</returns>
    public UnitDirection GetRightTurn(UnitDirection direction)
    {
        switch (direction)
        {
            case UnitDirection.North: return UnitDirection.East;
            case UnitDirection.East: return UnitDirection.South;
            case UnitDirection.South: return UnitDirection.West;
            case UnitDirection.West: return UnitDirection.North;
            default: return UnitDirection.South;
        }
    }

    /// <summary>
    /// Given a direction, gets the direction turned 90deg anti-clockwise.
    /// </summary>
    /// <param name="direction">The direction to turn left from.</param>
    /// <returns>The left-turned direction.</returns>
    public UnitDirection GetLeftTurn(UnitDirection direction)
    {
        switch (direction)
        {
            case UnitDirection.North: return UnitDirection.West;
            case UnitDirection.East: return UnitDirection.North;
            case UnitDirection.South: return UnitDirection.East;
            case UnitDirection.West: return UnitDirection.South;
            default: return UnitDirection.South;
        }
    }

    /// <summary>
    /// Given a tile, direction and faction, check to see if there is valid unit of the opposing faction that has not been captured yet in front of the given tile.
    /// </summary>
    /// <param name="tile">The tile to do the check from.</param>
    /// <param name="direction">The direction in which to do the check.</param>
    /// <param name="faction">The faction of the robot that would attempt to capture at this point.</param>
    /// <param name="message">The feedback message that can be used in the info popup if not able to capture.</param>
    /// <returns>True if can capture, False if not</returns>
    public bool GetCapture(Tile tile, UnitDirection direction, Faction faction, out string message)
    {
        Tile tileToCapture = null;
        message = null;
        switch (direction)
        {
            case UnitDirection.South:
                tileToCapture = GridManager.Instance.GetTileSouthOfPosition(tile.transform.position);
                break;
            case UnitDirection.West:
                tileToCapture = GridManager.Instance.GetTileWestOfPosition(tile.transform.position);
                break;
            case UnitDirection.North:
                tileToCapture = GridManager.Instance.GetTileNorthOfPosition(tile.transform.position);
                break;
            case UnitDirection.East:
                tileToCapture = GridManager.Instance.GetTileEastOfPosition(tile.transform.position);
                break;
            default: break;
        }
        if (tileToCapture is not null && tileToCapture.OccupiedUnit is not null)
        {
            if (tileToCapture.OccupiedUnit.Faction != faction)
            {
                //Can player capture player??
                if(tileToCapture.OccupiedUnit is BaseBuilding)
                {                    
                    if (!tileToCapture.Captured)
                    {
                        if (!((BaseBuilding)tileToCapture.OccupiedUnit).IsShielded)
                        {
                            return true;
                        }
                        else { message = "Can not capture shielded building"; }
                    }
                    else
                    {
                        message = "The building has already been captured";
                    }
                }
                else { message = "Can not capture opponent's robot"; }                
            }
            else
            {
                if (!tileToCapture.UnitIsIgnored())
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

    /// <summary>
    /// Attempts to move a given robot forward.
    /// </summary>
    /// <param name="robot">The robot to move.</param>
    private void moveForward(BaseRobot robot) {
        Debug.Log("Attempting move from Tile: " + robot.transform.position.ToString());
        Tile tileForward;
        switch (robot.direction) {
            case UnitDirection.South:
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
            case UnitDirection.West:
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
            case UnitDirection.North:
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
            case UnitDirection.East:
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

    /// <summary>
    /// Attempts to move a given robot backwards.
    /// </summary>
    /// <param name="robot">The robot to move.</param>
    private void moveBackwards(BaseRobot robot)
    {
        Debug.Log("Attempting move from Tile: " + robot.transform.position.ToString());
        Tile tileBackwards;
        switch (robot.direction)
        {            
            case UnitDirection.South:
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
            case UnitDirection.West:
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
            case UnitDirection.North:
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
            case UnitDirection.East:
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

    /// <summary>
    /// Turns a given robot 90deg clockwise.
    /// </summary>
    /// <param name="robot">The robot to turn right.</param>
    private void turnRight(BaseRobot robot)
    {
        robot.direction = GetRightTurn(robot.direction);
        Debug.Log("Robot turned right to " + robot.direction);
    }

    /// <summary>
    /// Turns a given robot 90deg anti-clockwise.
    /// </summary>
    /// <param name="robot">The robot to turn left.</param>
    private void turnLeft(BaseRobot robot)
    {
        robot.direction = GetLeftTurn(robot.direction);
        Debug.Log("Robot turned left to " + robot.direction);
    }

    /// <summary>
    /// Given a robot, attempt to capture the tile in front of the robot.
    /// </summary>
    /// <param name="robot">The robot that will attempt to capture.</param>
    private void attemptCapture(BaseRobot robot)
    {
        Debug.Log("Attempting to capture from Tile: " + robot.transform.position.ToString());
        Tile tileToCapture = null;
        string messageDirection = "";
        switch (robot.direction)
        {
            case UnitDirection.South:
                tileToCapture = GridManager.Instance.GetTileSouthOfPosition(robot.OccupiedTile.transform.position);
                messageDirection = " in South direction";                
                break;
            case UnitDirection.West:
                tileToCapture = GridManager.Instance.GetTileWestOfPosition(robot.OccupiedTile.transform.position);
                messageDirection = " in West direction";
                break;
            case UnitDirection.North:
                tileToCapture = GridManager.Instance.GetTileNorthOfPosition(robot.OccupiedTile.transform.position);
                messageDirection = " in North direction";
                break;
            case UnitDirection.East:
                tileToCapture = GridManager.Instance.GetTileEastOfPosition(robot.OccupiedTile.transform.position);
                messageDirection = " in East direction";
                break;
            default: break;
        }
        if (tileToCapture != null && tileToCapture.OccupiedUnit != null)
        {
            if (tileToCapture.OccupiedUnit.Faction != robot.Faction)
            {
                //Can player capture player??
                if (tileToCapture.OccupiedUnit is BaseBuilding)
                {
                    if (!tileToCapture.Captured)
                    {
                        if (!((BaseBuilding)tileToCapture.OccupiedUnit).IsShielded)
                        {
                            Debug.Log("Robot capturing in South direction");
                            tileToCapture.CaptureBuilding(robot.Faction);
                            if(robot.Faction == Faction.Player)
                            {
                                _playerAmmo++;
                            }
                            else _enemyAmmo++;
                        }
                        else { Debug.Log("Can not capture shielded building" + messageDirection); }
                    }
                    else
                    {
                        Debug.Log("The building has already been captured" + messageDirection);
                    }
                }
                else { Debug.Log("Can not capture opponent's robot" + messageDirection); }

            }
            else
            {
                Debug.Log("Robot can't capture own piece");
            }
        }
        else
        {
            Debug.Log("Robot could not capture" + messageDirection);
        }
    }

    public Tile PreviewShotBeam(Faction faction)
    {
        BaseRobot robot = _playerRobot;
        if (faction == Faction.Enemy)
        {
            robot = _enemyRobot;
            _enemyAmmo--;
        }
        else
        {
            _playerAmmo--;
        }
        Vector2 checkCollision = robot.transform.position;
        Tile tileToCheck;
        do
        {
            switch (robot.direction)
            {
                case UnitDirection.South:
                    checkCollision.y--; break;
                case UnitDirection.West:
                    checkCollision.x--; break;
                case UnitDirection.North:
                    checkCollision.y++; break;
                case UnitDirection.East:
                    checkCollision.x++; break;
                default: break;
            }
            tileToCheck = GridManager.Instance.GetTileAtPosition(checkCollision);
        } while (tileToCheck is not null && tileToCheck.Walkable);
        
        return tileToCheck;
    }

    /// <summary>
    /// Given a robot, shoot a laser beam in front of the robot that travels until it hits an obstacle or goes offscreen/grid.
    /// </summary>
    /// <param name="robot">The robot that will shoot.</param>
    public void ShootBeam(Faction faction)
    {
        BaseRobot robot = _playerRobot;
        /*if (faction == Faction.Enemy)
        {
            robot = _enemyRobot;
            _enemyAmmo--;
        }
        else
        {
            _playerAmmo--;
        }*/
        Debug.Log("Shooting from Tile: " + robot.transform.position.ToString() + " in direction of " + robot.direction);
        Vector2 checkCollision = robot.transform.position;
        Tile tileToCheck;
        do
        {
            switch (robot.direction)
            {
                case UnitDirection.South:
                    checkCollision.y--; break;
                case UnitDirection.West:
                    checkCollision.x--; break;
                case UnitDirection.North:
                    checkCollision.y++; break;
                case UnitDirection.East:
                    checkCollision.x++; break;
                default: break;
            }
            tileToCheck = GridManager.Instance.GetTileAtPosition(checkCollision);
        } while (tileToCheck is not null && tileToCheck.Walkable);
        if( tileToCheck == null ) { 
            //Laser beam goes off map/grid
        }
        else
        {
            //Laser beam hits obstacle
            if( tileToCheck is MountainTile)
            {
                //Destroy mountain, and add to playable tiles?

            }
            else
            {
                tileToCheck.OccupiedUnit.GetShot(faction);
            }
        }        
    }

    public void UndoShot(Faction faction)
    {
        if ( faction == Faction.Enemy )
        {
            _enemyAmmo++;
        }
        else
        {
             _playerAmmo++;
        }
    }

    public bool HasAmmo(Faction faction)
    {
        if(faction == Faction.Player)
        {
            return _playerAmmo > 0;
        }
        else return _enemyAmmo > 0;
    }

    public bool isStunnedRobot(Faction faction)
    {
        if (faction == Faction.Player)
        {
            return _playerRobot.IsStunned;
        }
        else return _enemyRobot.IsStunned;
    }

    public void DebugAmmoCount()
    {
        Debug.Log("Player ammo: " +  _playerAmmo);
        Debug.Log("Enemy ammo: " +  _enemyAmmo);
    }
}
