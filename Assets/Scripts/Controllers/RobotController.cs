using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class RobotController : NetworkBehaviour
{
    /// <summary>
    /// This Singleton class handles all movement that occurs for the robots.
    /// </summary>
    [SerializeField] private int _startingAmmoCount;
    [SerializeField] private float _movementTime;
    public static RobotController Instance;
    private BaseRobot _playerRobot;
    private NetworkVariable<int> _playerAmmo = new NetworkVariable<int>(0); //still need to indicate in UI
    private BaseRobot _enemyRobot;
    private NetworkVariable<int> _enemyAmmo = new NetworkVariable<int>(0);

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Obtains a reference to the player's robot
    /// </summary>
    /// <param name="player">The player robot</param>
    public void InitPlayerOnServer(BaseRobot player)
    {
        Debug.Log("Initing Player");
        _playerRobot = player;
        _playerAmmo.Value = _startingAmmoCount;
        _playerAmmo.SetDirty(true);
    }

    /// <summary>
    /// Obtains a reference to the enemy's robot
    /// </summary>
    /// <param name="enemy"></param>
    public void InitEnemyOnServer(BaseRobot enemy)
    {
        Debug.Log("Initing Enemy");
        _enemyRobot = enemy;
        _enemyAmmo.Value = _startingAmmoCount;
        _enemyAmmo.SetDirty(true);
    }

    /// <summary>
    /// Gets the Tile that the robot of a given faction is positioned on
    /// </summary>
    /// <param name="faction">The faction that the Robot belongs to: Player or Enemy</param>
    /// <returns>The tile that is occupied by the robot</returns>
    public Vector2 GetRobotPositionForServer(Faction faction)
    {
        return faction == Faction.Player ? _playerRobot.OccupiedTile.transform.position : _enemyRobot.OccupiedTile.transform.position;
    }

    /// <summary>
    /// Gets the direction that the robot of a given faction is facing.
    /// </summary>
    /// <param name="faction">The faction that the Robot belongs to: Player or Enemy</param>
    /// <returns>BaseRobot.Direction - the direction the robot is facing towards</returns>
    public UnitDirection GetRobotDirectionForServer(Faction faction)
    {
        return faction == Faction.Player ? _playerRobot.direction.Value : _enemyRobot.direction.Value;
    }

    /// <summary>
    /// Given a tile and a direction to check movement from, check if the tile infront of it will be walkable and return it. <br />
    /// </summary>
    /// <param name="position">The tile from which to check.</param>
    /// <param name="direction">The direction in which the check should take place.</param>
    /// <param name="message">The feedback message that the player will get in the info popup if the tile in front is not walkable.</param>
    /// <returns>The tile in front of the given tile if it is walkable, else returns null</returns>
    public Tile GetLocalForward(Vector2 position, UnitDirection direction)
    {
        Tile tileForward;
        switch (direction)
        {
            case UnitDirection.South:
                tileForward = GridManager.Instance.GetTileSouth(position, TileManager.Instance.GetLocalPlayableTiles());
                if (tileForward is not null && tileForward.Walkable)
                {
                    return tileForward;
                }
                //message = "Tile forward to South is not Walkable";
                break;
            case UnitDirection.West:
                tileForward = GridManager.Instance.GetTileWest(position, TileManager.Instance.GetLocalPlayableTiles());
                if (tileForward is not null && tileForward.Walkable)
                {
                    return tileForward;
                }
                //message = "Tile forward to West is not Walkable";
                break;
            case UnitDirection.North:
                tileForward = GridManager.Instance.GetTileNorth(position, TileManager.Instance.GetLocalPlayableTiles());
                if (tileForward is not null && tileForward.Walkable)
                {
                    return tileForward;
                }
                //message = "Tile forward to North is not Walkable";
                break;
            case UnitDirection.East:
                tileForward = GridManager.Instance.GetTileEast(position, TileManager.Instance.GetLocalPlayableTiles());
                if (tileForward is not null && tileForward.Walkable)
                {
                    return tileForward;
                }
                //message = "Tile forward to East is not Walkable";
                break;
            default: return null;
        }
        return null;
    }

    /// <summary>
    /// Given a tile and a direction to check movement from, check if the tile behind it will be walkable and return it. <br />
    /// </summary>
    /// <param name="position">The tile from which to check.</param>
    /// <param name="direction">The direction in which the check should take place.</param>
    /// <param name="message">The feedback message that the player will get in the info popup if the tile behind is not walkable.</param>
    /// <returns>The tile behind the given tile if it is walkable, else returns null</returns>
    public Tile GetLocalBackwards(Vector2 position, UnitDirection direction)
    {
        Tile tileBackwards;
        switch (direction)
        {
            case UnitDirection.South:
                tileBackwards = GridManager.Instance.GetTileNorth(position, TileManager.Instance.GetLocalPlayableTiles());
                if (tileBackwards is not null && tileBackwards.Walkable)
                {
                    return tileBackwards;
                }
                //message = "Tile backwards from South is not Walkable";
                break;
            case UnitDirection.West:
                tileBackwards = GridManager.Instance.GetTileEast(position, TileManager.Instance.GetLocalPlayableTiles());
                if (tileBackwards is not null && tileBackwards.Walkable)
                {
                    return tileBackwards;
                }
                //message = "Tile backwards from West is not Walkable";
                break;
            case UnitDirection.North:
                tileBackwards = GridManager.Instance.GetTileSouth(position, TileManager.Instance.GetLocalPlayableTiles());
                if (tileBackwards is not null && tileBackwards.Walkable)
                {
                    return tileBackwards;
                }
                //message = "Tile backwards from North is not Walkable";
                break;
            case UnitDirection.East:
                tileBackwards = GridManager.Instance.GetTileWest(position, TileManager.Instance.GetLocalPlayableTiles());
                if (tileBackwards is not null && tileBackwards.Walkable)
                {
                    return tileBackwards;
                }
                //message = "Tile backwards from East is not Walkable";
                break;
            default: return null;
        }
        return null;
    }

    /// <summary>
    /// Given a tile, direction and faction, check to see if there is valid unit of the opposing faction that has not been captured yet in front of the given tile.
    /// </summary>
    /// <param name="position">The tile to do the check from.</param>
    /// <param name="direction">The direction in which to do the check.</param>
    /// <param name="faction">The faction of the robot that would attempt to capture at this point.</param>
    /// <param name="message">The feedback message that can be used in the info popup if not able to capture.</param>
    /// <returns>True if can capture, False if not</returns>
    public bool GetLocalCapture(Vector2 position, UnitDirection direction, Faction faction)
    {
        Tile tileToCapture = null;
        switch (direction)
        {
            case UnitDirection.South:
                tileToCapture = GridManager.Instance.GetTileSouth(position, TileManager.Instance.GetLocalPlayableTiles());
                break;
            case UnitDirection.West:
                tileToCapture = GridManager.Instance.GetTileWest(position, TileManager.Instance.GetLocalPlayableTiles());
                break;
            case UnitDirection.North:
                tileToCapture = GridManager.Instance.GetTileNorth(position, TileManager.Instance.GetLocalPlayableTiles());
                break;
            case UnitDirection.East:
                tileToCapture = GridManager.Instance.GetTileEast(position, TileManager.Instance.GetLocalPlayableTiles());
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
                        if (!((BaseBuilding) tileToCapture.OccupiedUnit).IsShielded)
                        {
                            return true;
                        }
                        else { 
                            //message = "Can not capture shielded building"; 
                        }
                    }
                    else
                    {
                        //message = "The building has already been captured";
                    }
                }
                else { 
                    //message = "Can not capture opponent's robot";
                    }                
            }
            else
            {
                if (!tileToCapture.UnitIsIgnored())
                {
                    //message = "You can not capture your own building";
                }
            }
        }
        else
        {
            //message = "There is nothing to capture";
        }
        return false;
    }



    /// <summary>
    /// Attempts to move a given robot forward.
    /// </summary>
    /// <param name="robot">The robot to move.</param>
    private IEnumerator moveForward(BaseRobot robot) {
        Debug.Log("Attempting move from Tile: " + robot.transform.position.ToString());
        Tile tileForward;
        switch (robot.direction.Value) {
            case UnitDirection.South:
                tileForward = GridManager.Instance.GetTileSouthOfPositionOnServer(robot.OccupiedTile.transform.position);
                if (tileForward is not null && tileForward.Walkable) {
                    robot.SetWalking(true);
                    tileForward.SetUnit(robot);
                    robot.SetUnitOnTileClientRpc(tileForward.transform.position);
                    yield return new WaitForSeconds(_movementTime);
                    robot.SetWalking(false);
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
                tileForward = GridManager.Instance.GetTileWestOfPositionOnServer(robot.OccupiedTile.transform.position);
                if (tileForward is not null && tileForward.Walkable)
                {
                    robot.SetWalking(true);
                    tileForward.SetUnit(robot);
                    robot.SetUnitOnTileClientRpc(tileForward.transform.position);
                    yield return new WaitForSeconds(_movementTime);
                    robot.SetWalking(false);
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
                tileForward = GridManager.Instance.GetTileNorthOfPositionOnServer(robot.OccupiedTile.transform.position);
                if (tileForward is not null && tileForward.Walkable)
                {
                    robot.SetWalking(true);
                    tileForward.SetUnit(robot);
                    robot.SetUnitOnTileClientRpc(tileForward.transform.position);
                    yield return new WaitForSeconds(_movementTime);
                    robot.SetWalking(false);
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
                tileForward = GridManager.Instance.GetTileEastOfPositionOnServer(robot.OccupiedTile.transform.position);
                if (tileForward is not null && tileForward.Walkable)
                {
                    robot.SetWalking(true);
                    tileForward.SetUnit(robot);
                    robot.SetUnitOnTileClientRpc(tileForward.transform.position);
                    yield return new WaitForSeconds(_movementTime);
                    robot.SetWalking(false);
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
    private IEnumerator moveBackwards(BaseRobot robot)
    {
        Debug.Log("Attempting move from Tile: " + robot.transform.position.ToString());
        Tile tileBackwards;
        switch (robot.direction.Value)
        {            
            case UnitDirection.South:
                tileBackwards = GridManager.Instance.GetTileNorthOfPositionOnServer(robot.OccupiedTile.transform.position);
                if (tileBackwards is not null && tileBackwards.Walkable)
                {
                    robot.SetWalking(true);
                    tileBackwards.SetUnit(robot);
                    robot.SetUnitOnTileClientRpc(tileBackwards.transform.position);
                    yield return new WaitForSeconds(_movementTime);
                    robot.SetWalking(false);
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
                tileBackwards = GridManager.Instance.GetTileEastOfPositionOnServer(robot.OccupiedTile.transform.position);
                if (tileBackwards is not null && tileBackwards.Walkable)
                {
                    robot.SetWalking(true);
                    tileBackwards.SetUnit(robot);
                    robot.SetUnitOnTileClientRpc(tileBackwards.transform.position);
                    yield return new WaitForSeconds(_movementTime);
                    robot.SetWalking(false);
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
                tileBackwards = GridManager.Instance.GetTileSouthOfPositionOnServer(robot.OccupiedTile.transform.position);
                if (tileBackwards is not null && tileBackwards.Walkable)
                {
                    robot.SetWalking(true);
                    tileBackwards.SetUnit(robot);
                    robot.SetUnitOnTileClientRpc(tileBackwards.transform.position);
                    yield return new WaitForSeconds(_movementTime);
                    robot.SetWalking(false);
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
                tileBackwards = GridManager.Instance.GetTileWestOfPositionOnServer(robot.OccupiedTile.transform.position);
                if (tileBackwards is not null && tileBackwards.Walkable)
                {
                    robot.SetWalking(true);
                    tileBackwards.SetUnit(robot);
                    robot.SetUnitOnTileClientRpc(tileBackwards.transform.position);
                    yield return new WaitForSeconds(_movementTime);
                    robot.SetWalking(false);
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
    private IEnumerator turnRight(BaseRobot robot)
    {
        robot.SetWalking(true);
        yield return new WaitForSeconds(_movementTime / 3);
        robot.direction.Value = UnitManager.Instance.GetRightTurn(robot.direction.Value);
        AnimationManager.Instance.UpdateRobotAnimation(robot);
        yield return new WaitForSeconds(_movementTime / 3);
        robot.SetWalking(false);
        yield return new WaitForSeconds(_movementTime / 3);
        //robot.transform.Rotate(0, 0, -90);
        Debug.Log("Robot turned right to " + robot.direction.Value);
    }

    /// <summary>
    /// Turns a given robot 90deg anti-clockwise.
    /// </summary>
    /// <param name="robot">The robot to turn left.</param>
    private IEnumerator turnLeft(BaseRobot robot)
    {
        robot.SetWalking(true);
        yield return new WaitForSeconds(_movementTime / 3);
        robot.direction.Value = UnitManager.Instance.GetLeftTurn(robot.direction.Value);
        AnimationManager.Instance.UpdateRobotAnimation(robot);
        yield return new WaitForSeconds(_movementTime / 3);
        robot.SetWalking(false);
        yield return new WaitForSeconds(_movementTime / 3);
        
        //robot.ChangeAnimationState()
        //robot.transform.Rotate(0, 0, 90);
        Debug.Log("Robot turned left to " + robot.direction.Value);
    }

    /// <summary>
    /// Given a robot, attempt to capture the tile in front of the robot.
    /// </summary>
    /// <param name="robot">The robot that will attempt to capture.</param>
    private IEnumerator attemptCapture(BaseRobot robot)
    {        
        yield return new WaitForSeconds(_movementTime/2);
        Debug.Log("Attempting to capture from Tile: " + robot.transform.position.ToString());
        Tile tileToCapture = null;
        string messageDirection = "";
        switch (robot.direction.Value)
        {
            case UnitDirection.South:
                tileToCapture = GridManager.Instance.GetTileSouthOfPositionOnServer(robot.OccupiedTile.transform.position);
                messageDirection = " in South direction";                
                break;
            case UnitDirection.West:
                tileToCapture = GridManager.Instance.GetTileWestOfPositionOnServer(robot.OccupiedTile.transform.position);
                messageDirection = " in West direction";
                break;
            case UnitDirection.North:
                tileToCapture = GridManager.Instance.GetTileNorthOfPositionOnServer(robot.OccupiedTile.transform.position);
                messageDirection = " in North direction";
                break;
            case UnitDirection.East:
                tileToCapture = GridManager.Instance.GetTileEastOfPositionOnServer(robot.OccupiedTile.transform.position);
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
                            Debug.Log("Robot capturing" + messageDirection);
                            tileToCapture.CaptureBuilding(robot.Faction);
                            if (robot.Faction == Faction.Player)
                            {
                                _playerAmmo.Value++;
                                _playerAmmo.SetDirty(true);
                            }
                            else
                            {
                                _enemyAmmo.Value++;
                                _enemyAmmo.SetDirty(true);
                            }
                            AnimationManager.Instance.PlayParticleClientRpc(tileToCapture.transform.position);
                            SoundManager.Instance.PlayCaptureBuildingSound(tileToCapture.transform.position);
                            tileToCapture.SetUnit(robot);
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
        yield return new WaitForSeconds(_movementTime / 2);
    }

    public Vector2 PreviewShotBeam(Vector2 startPos, UnitDirection direction)
    {
        Vector2 checkCollision = startPos;
        Tile tileToCheck;
        do
        {
            switch (direction)
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
            tileToCheck = GridManager.Instance.GetTileAtPos(checkCollision, TileManager.Instance.GetLocalPlayableTiles());
        } while (tileToCheck is not null && tileToCheck.Walkable);
        
        return checkCollision;
    }

    /// <summary>
    /// Given a robot, shoot a laser beam in front of the robot that travels until it hits an obstacle or goes offscreen/grid.
    /// </summary>
    /// <param name="robot">The robot that will shoot.</param>
    public IEnumerator ShootBeamOnServer(BaseRobot robot)
    {
        if (robot.Faction == Faction.Player)
        {
            _playerAmmo.Value--;
            _playerAmmo.SetDirty(true);
        }
        else
        {
            _enemyAmmo.Value--;
            _enemyAmmo.SetDirty(true);
        }
        Debug.Log("Shooting from Tile: " + robot.transform.position.ToString() + " in direction of " + robot.direction.Value);
        Vector2 checkCollision = robot.transform.position;
        Tile tileToCheck;
        do
        {
            switch (robot.direction.Value)
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
            tileToCheck = GridManager.Instance.GetTileAtPositionOnServer(checkCollision);
        } while (tileToCheck is not null && tileToCheck.Walkable);

        SoundManager.Instance.PlayShootBeamSound(robot.transform.position);
        yield return new WaitForSeconds(_movementTime / 2);
        if ( tileToCheck == null ) { 
            //Laser beam goes off map/grid
        }
        else
        {
            //Laser beam hits obstacle
            if( tileToCheck is UnwalkableTile)
            {
                //Destroy mountain, and add to playable tiles?

            }
            else
            {
                //do shoot animation
                tileToCheck.OccupiedUnit.GetShot(robot.Faction);
                //tileToCheck.OccupiedUnit.GetShotClientRpc(faction); //TODO: REWORK
            }
        }        
        yield return new WaitForSeconds(_movementTime/2);
    }
    public bool HasAmmo(Faction faction)
    {
        if(faction == Faction.Player)
        {
            return _playerAmmo.Value > 0;
        }
        else return _enemyAmmo.Value > 0;
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
        Debug.Log("Player ammo: " + _playerAmmo.Value);
        Debug.Log("Enemy ammo: " + _enemyAmmo.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SubmitMovesServerRpc(Token token1, Token token2, Token token3, Token token4)
    {
        //TODO: Check if received from correct server
        //TODO: Update clients that are not receiver to move
        //TODO: Insert Animation calls to clients inside of methods above
        Token[] _tokens = new Token[4];
        _tokens[0] = token1;
        _tokens[1] = token2;
        _tokens[2] = token3;
        _tokens[3] = token4;

        BaseRobot robot = GameManager.Instance.Gamestate.Value == GameState.PlayerTurn ? _playerRobot : _enemyRobot;
        Debug.Log("Pre-execution of moves on server");
        StartCoroutine(executeMoves(_tokens, robot));
    }

    private IEnumerator executeMoves(Token[] tokens, BaseRobot robot)
    {
        for (int i = 0; i < 4; i++)
        {
            Debug.LogWarning("Execution of input " + i);
            Coroutine coroutine = null;
            switch (tokens[i])
            {
                case Token.Forward:
                    coroutine = StartCoroutine(moveForward(robot));
                    break;
                case Token.Backward:
                    coroutine = StartCoroutine(moveBackwards(robot));
                    break;
                case Token.Left:
                    coroutine = StartCoroutine(turnLeft(robot));
                    break;
                case Token.Right:
                    coroutine = StartCoroutine(turnRight(robot));
                    break;
                case Token.Capture:
                    coroutine = StartCoroutine(attemptCapture(robot));
                    break;
                case Token.Shoot:
                    coroutine = StartCoroutine(ShootBeamOnServer(robot));
                    break;
                case Token.Empty:
                    Debug.Log("EMPTY TOKEN INPUT ON SERVER");
                    break;
                default: break;
            }
            if (coroutine == null) break;
            Debug.LogWarning("Yielding to coroutine now");
            yield return coroutine;
            Debug.LogWarning("Finished yielding");
        }
        if (!UnitManager.Instance.CheckIsGameOver())
        {
            if (GameManager.Instance.Gamestate.Value == GameState.PlayerTurn)
            {
                GameManager.Instance.ChangeStateServerRpc(GameState.EnemyTurn);
            }
            else
            {
                GameManager.Instance.ChangeStateServerRpc(GameState.PlayerTurn);
            }
        }
    }
}
