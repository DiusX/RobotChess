
using Unity.Netcode;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using Color = UnityEngine.Color;
using Image = UnityEngine.UI.Image;

public class InputController : NetworkBehaviour
{
    /// <summary>
    /// This Singleton class handles all the input commands from the player.
    /// </summary>
    public static InputController Instance;
    [SerializeField] private GameObject _buttonInputContainer;
    [SerializeField] private GameObject[] _inputs;
    [SerializeField] private Button _buttonForward, _buttonBackwards, _buttonLeft, _buttonRight, _buttonCapture, _buttonUndo, _buttonCommit, _buttonShoot;
    [SerializeField] private RobotController _playerController;
    private GameObject _robotGhost;

    private Token[] _tokens;
    private int _index;
    private Vector2 _position;
    private Tile _startTile;
    private UnitDirection _direction;
    private bool _isStunned;
    private bool _hasAmmo;
    private bool _hasAlreadyShot;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {        
        _tokens = new Token[_inputs.Length];
    }

    /// <summary>
    /// Initialises the temp robot. Used at the start of each turn.
    /// </summary>
    /// <param name="position">The position the robot will start at.</param>
    /// <param name="direction">The direction the robot will face.</param>
    [ClientRpc]
    public void InitTempRobotClientRpc(Vector2 position, UnitDirection direction, bool isStunned, bool hasAmmo)
    {
        Debug.Log("Initing Temp Robot");
        _position = position;
        if (IsServer) {
            _startTile = GridManager.Instance.GetTileAtPositionOnServer(position);
        }
        else _startTile = TileManager.Instance.GetLocalPlayableTile(position);

        _direction = direction;
        _isStunned = isStunned;
        _hasAmmo = hasAmmo;
        Debug.LogWarning("IS STUNNED: " + isStunned);
        _hasAlreadyShot = false;
        if (_robotGhost != null)
        {
            Destroy(_robotGhost);
        }
        _robotGhost = new GameObject
        {
            name = "Robot Ghost"
        };
        _robotGhost.AddComponent<SpriteRenderer>();
        _robotGhost.GetComponent<SpriteRenderer>().sortingOrder = 3;
        /*if (GameManager.Instance.Gamestate.Value == GameState.PlayerTurn) _robotGhost.GetComponent<SpriteRenderer>().color = Color.cyan;
        else _robotGhost.GetComponent<SpriteRenderer>().color = Color.red;
        Color alphaColor = _robotGhost.GetComponent<SpriteRenderer>().color;
        alphaColor.a = 0.5f;
        _robotGhost.GetComponent<SpriteRenderer>().color = alphaColor;*/
        _robotGhost.GetComponent<SpriteRenderer>().color = Color.yellow;

        _buttonInputContainer.SetActive(true);
        clearTokenSelection();
        updateRobotGhost();
        updateTokenAvailability();
        RobotController.Instance.DebugAmmoCount();
    }

    /// <summary>
    /// Adds forward token to the token list.
    /// </summary>
    public void MoveForwardToken()
    {
        AddToken(Token.Forward);
    }

    /// <summary>
    /// Adds backwards token to the token list.
    /// </summary>
    public void MoveBackwardsToken()
    {
        AddToken(Token.Backward);
    }

    /// <summary>
    /// Adds turn right token to the token list.
    /// </summary>
    public void TurnRightToken()
    {
        AddToken(Token.Right);
    }

    /// <summary>
    /// Adds turn left token to the token list.
    /// </summary>
    public void TurnLeftToken()
    {
        AddToken(Token.Left);
    }

    /// <summary>
    /// Adds capture token to the token list.
    /// </summary>
    public void CaptureToken()
    {
        AddToken(Token.Capture);
    }

    /// <summary>
    /// Adds shoot token to the token list.
    /// </summary>
    public void ShootToken()
    {
        AddToken(Token.Shoot);
    }

    /// <summary>
    /// Commits current token list.
    /// </summary>
    public void CommitToken()
    {
        CommitMoves();
    }

    /// <summary>
    /// Removes the last token added to the token list.
    /// </summary>
    public void UndoToken()
    {
        undoMove();
    }

    /// <summary>
    /// Skips the current turn by sacrifising a random building piece.
    /// </summary>
    public void SkipTurnToken()
    {
        SkipTurn();
    }

    /// <summary>
    /// Removes the first and all non-stun tokens that follow from the input list.
    /// </summary>
    public void UndoInputOne()
    {
        undoMovesToIndex(0);
    }

    /// <summary>
    /// Removes the second and all non-stun tokens that follow from the input list.
    /// </summary>
    public void UndoInputTwo()
    {
        undoMovesToIndex(1);
    }

    /// <summary>
    /// If not stunned, removes the third and fourth tokens from the input list.
    /// </summary>
    public void UndoInputThree()
    {
        if(!_isStunned)
            undoMovesToIndex(2);
    }

    /// <summary>
    /// If not stunned, removes the fourth token from the input list.
    /// </summary>
    public void UndoInputFour()
    {
        if (!_isStunned)
            undoMovesToIndex(3);
    }

    /// <summary>
    /// Updates the ghost of a robot to reflect the correct state at a given time in the token input process.
    /// </summary>
    private void updateRobotGhost()
    {
        _robotGhost.transform.position = _position;
        //_robotGhost.transform.rotation = Quaternion.identity;
        switch (_direction)
        {
            case (UnitDirection.South):
                {
                    _robotGhost.GetComponent<SpriteRenderer>().sprite = SpriteManager.Instance.GetRobotSouthSprite();
                    _robotGhost.GetComponent<SpriteRenderer>().flipX = false;
                    break;
                }                
            case (UnitDirection.West):
                {
                    _robotGhost.GetComponent<SpriteRenderer>().sprite = SpriteManager.Instance.GetRobotWestSprite();
                    _robotGhost.GetComponent<SpriteRenderer>().flipX = true;
                    break;
                }                
            case (UnitDirection.North):
                {
                    _robotGhost.GetComponent<SpriteRenderer>().sprite = SpriteManager.Instance.GetRobotNorthSprite();
                    _robotGhost.GetComponent<SpriteRenderer>().flipX = false;
                    break;
                }                
            case (UnitDirection.East):
                {
                    _robotGhost.GetComponent<SpriteRenderer>().sprite = SpriteManager.Instance.GetRobotEastSprite();
                    _robotGhost.GetComponent<SpriteRenderer>().flipX = false;
                    break;
                }                
        }
    }

    /// <summary>
    /// Checks and updates the enabled state of tokens on screen to prevent users from inputting tokens that would be invalid.
    /// This is done by disabling the tokens (greyed out) that would break game logic if played, thus preventing the user from interacting with them.
    /// </summary>
    private void updateTokenAvailability()
    {
        if (_index == 0)
        {
            _buttonUndo.gameObject.SetActive(false);
        }
        else _buttonUndo.gameObject.SetActive(true);
        if (_index >= _inputs.Length - (_isStunned ? 2 : 0))
        {
            //All tokens disabled or offscreen -> show Commit token only
            _buttonForward.gameObject.SetActive(false); _buttonBackwards.gameObject.SetActive(false); _buttonLeft.gameObject.SetActive(false); _buttonRight.gameObject.SetActive(false); _buttonCapture.gameObject.SetActive(false); _buttonShoot.gameObject.SetActive(false); _buttonCommit.gameObject.SetActive(true);
            return;
        }
        _buttonForward.gameObject.SetActive(true); _buttonBackwards.gameObject.SetActive(true); _buttonLeft.gameObject.SetActive(true); _buttonRight.gameObject.SetActive(true); _buttonCapture.gameObject.SetActive(true); _buttonShoot.gameObject.SetActive(true); _buttonCommit.gameObject.SetActive(false);

        #region Token.Forward
        if (_index > 0 && _tokens[_index - 1] == Token.Backward)
        {
            //Can't cancel out moves
            _buttonForward.gameObject.SetActive(false);
        }
        else if (_playerController.GetLocalForward(_position, _direction) == null)
        {
            //No tile forward found
            _buttonForward.gameObject.SetActive(false);
        }
        #endregion

        #region Token.Backwards
        if (_index > 0 && _tokens[_index - 1] == Token.Forward)
        {
            //Can't cancel out moves
            _buttonBackwards.gameObject.SetActive(false);
        }
        //implement movement
        else if (_playerController.GetLocalBackwards(_position, _direction) == null)
        {
            //No tile backwards found
            _buttonBackwards.gameObject.SetActive(false);
        }
        #endregion

        #region Token.Left
        if (_index > 0 && _tokens[_index - 1] == Token.Right)
        {
            //Can't cancel out moves
            _buttonLeft.gameObject.SetActive(false);
        }
        else if (_index > 1 && _tokens[_index - 1] == Token.Left && _tokens[_index - 2] == Token.Left)
        {
            //Can't have more than 2 of same turn tokens after each other
            _buttonLeft.gameObject.SetActive(false);
        }
        #endregion

        #region Token.Right
        if (_index > 0 && _tokens[_index - 1] == Token.Left)
        {
            //Can't cancel out moves
            _buttonRight.gameObject.SetActive(false);
        }
        else if (_index > 1 && _tokens[_index - 1] == Token.Right && _tokens[_index - 2] == Token.Right)
        {
            //Can't have more than 2 of same turn tokens after each other
            _buttonRight.gameObject.SetActive(false);
        }
        #endregion

        #region Token.Capture
        if (_index != 3 - (_isStunned ? 2 : 0))
        {
            //Not on last input token
            _buttonCapture.gameObject.SetActive(false);
        }
        else if (_playerController.GetLocalCapture(_position, _direction, GameManager.Instance.Gamestate.Value == GameState.PlayerTurn ? Faction.Player : Faction.Enemy) != null)
        {
            //Tile in front not captureable
            _buttonCapture.gameObject.SetActive(false);
        }
        #endregion

        #region Token.Shoot
        if (_hasAlreadyShot || !_hasAmmo)
        {
            //!RobotController.Instance.HasAmmo(GameManager.Instance.Gamestate.Value == GameState.PlayerTurn ? Faction.Player : Faction.Enemy)
            _buttonShoot.gameObject.SetActive(false);
        }
        #endregion
    }

    /// <summary>
    /// Checks and handles new token to see if valid in the context of the game logic and then adds it. <br />
    /// Provides feedback to user in info popup if unable to add the given token.
    /// </summary>
    /// <param name="token">The token to add.</param>
    private void AddToken(Token token)
    {
        string message;
        if (_index >= _inputs.Length - (_isStunned ? 2 : 0))
        {
            //MenuManager notify: Max 4 moves.
            message = "Max 4 tokens allowed. Try to Commit";
            TileManager.Instance.ShowInfoPopup(message);
            return;
        }
        switch (token)
        {
            case (Token.Forward):
                {
                    if (_index > 0 && _tokens[_index - 1] == Token.Backward)
                    {
                        //MenuManager notify: Can't cancel out moves
                        message = "Can not cancel out moves. Try to Undo";
                        TileManager.Instance.ShowInfoPopup(message);
                        break;
                    }
                    //implement movement
                    Tile forward = _playerController.GetLocalForward(_position, _direction);
                    if (forward == null)
                    {
                        //MenuManger notify: Unable to move forward
                        message = "There is something preventing forward movement at this point.";
        
                        TileManager.Instance.ShowInfoPopup(message);
                        return;
                    }
                    _position = forward.transform.position;
                    setToken(Token.Forward);
                    _index++;
                    break;
                }
            case (Token.Backward):
                {
                    if (_index > 0 && _tokens[_index - 1] == Token.Forward)
                    {
                        //MenuManager notify: Can't cancel out moves
                        message = "Can not cancel out moves. Try to Undo";
                        TileManager.Instance.ShowInfoPopup(message);
                        break;
                    }
                    //implement movement
                    Tile backwards = _playerController.GetLocalBackwards(_position, _direction);
                    if (backwards == null)
                    {
                        //MenuManger notify: Unable to move backwards
                        message = "There is something preventing backwards movement at this point.";

                        TileManager.Instance.ShowInfoPopup(message);
                        return;
                    }
                    _position = backwards.transform.position;
                    setToken(Token.Backward);
                    _index++;
                    break;
                }
            case (Token.Left):
                {
                    if (_index > 0 && _tokens[_index - 1] == Token.Right)
                    {
                        //MenuManager notify: Can't cancel out moves
                        message = "Can not cancel out moves. Try to Undo";
                        TileManager.Instance.ShowInfoPopup(message);
                        break;
                    }
                    if (_index > 1 && _tokens[_index - 1] == Token.Left && _tokens[_index - 2] == Token.Left)
                    {
                        //MenuManager notify: Can't have more than 2 of same tokens after each other
                        message = "Can not use token more than 2 times in a row. Try moving.";
                        TileManager.Instance.ShowInfoPopup(message);
                        break;
                    }
                    //implement movement
                    _direction = UnitManager.Instance.GetLeftTurn(_direction);
                    setToken(Token.Left);
                    _index++;
                    break;
                }
            case (Token.Right):
                {
                    if (_index > 0 && _tokens[_index - 1] == Token.Left)
                    {
                        //MenuManager notify: Can't cancel out moves
                        message = "Can not cancel out moves. Try to Undo";
                        TileManager.Instance.ShowInfoPopup(message);
                        break;
                    }
                    if (_index > 1 && _tokens[_index - 1] == Token.Right && _tokens[_index - 2] == Token.Right)
                    {
                        //MenuManager notify: Can't have more than 2 of same tokens after each other
                        message = "Can not use token more than 2 times in a row. Try moving.";
                        TileManager.Instance.ShowInfoPopup(message);
                        break;
                    }
                    //implement movement
                    _direction = UnitManager.Instance.GetRightTurn(_direction);
                    setToken(Token.Right);
                    _index++;
                    break;
                }
            case (Token.Capture):
                {
                    if (_index != 3 - (_isStunned ? 2 : 0))
                    {
                        //MenuManager notify: Capture has to be on last move
                        message = "Capture has to be on the last move.";
                        TileManager.Instance.ShowInfoPopup(message);
                        break;
                    }
                    //implement movement
                    Tile tileToCapture = _playerController.GetLocalCapture(_position, _direction, GameManager.Instance.Gamestate.Value == GameState.PlayerTurn ? Faction.Player : Faction.Enemy);
                    if (tileToCapture == null)
                    {
                        //MenuManager notify: Can't capture
                        message = "Can not perform capture here.";

                        TileManager.Instance.ShowInfoPopup(message);
                        return;
                    }
                    _position = tileToCapture.transform.position;
                    setToken(Token.Capture);
                    _index++;
                    break;
                }
            case (Token.Shoot):
                {
                    Vector2 positionToShootTo = RobotController.Instance.PreviewShotBeam(_position, _direction);
                    //indicate shooting animation
                    setToken(Token.Shoot);
                    _index++;
                    _hasAlreadyShot = true;
                    break;
                }
            default: break;
        }
        updateRobotGhost();
        updateTokenAvailability();
    }

    /// <summary>
    /// Ends the current turn without commiting any moves.<br />
    /// Results in random building be destroyed.
    /// </summary>
    private void SkipTurn()
    {
        RobotController.Instance.SkipTurnServerRpc();
    }

    /// <summary>
    /// Checks and handles the attempt to commit the tokens that are currently in the token list. This also end the current turn. <br />
    /// Provides feedback to user in info popup if there is something preventing the commit from occuring.
    /// </summary>
    private void CommitMoves()
    {
        //implement Commit
        if (_index != 4 - (_isStunned ? 2 : 0))
        {
            //MenuManager notify: 4 moves required
            string message = "4 tokens are required to be able to commit.";
            TileManager.Instance.ShowInfoPopup(message);
            return;
        }
        _buttonInputContainer.SetActive(false);
        Destroy(_robotGhost);
        RobotController.Instance.SubmitMovesServerRpc(_tokens[0], _tokens[1], _tokens[2], _tokens[3]);
    }

    private void undoMovesToIndex(int index)
    {
        while (_index > index) { 
            undoMove();
        }
    }

    /// <summary>
    /// Checks and handles removing a token from the token list. <br />
    /// This will redo movement on the temporary robot in the opposite, in order to return the temp robot to its state before the last token.
    /// </summary>
    private void undoMove()
    {
        if (_index > 0)
        {
            _startTile.SetIgnoreUnit(true); //This is to prevent unit from seeing itself in the movement calculations.
            //undo movement
            switch (_tokens[--_index])
            {
                case Token.Forward:
                    _position = _playerController.GetLocalBackwards(_position, _direction).transform.position;
                    break;
                case Token.Backward:
                    _position = _playerController.GetLocalForward(_position, _direction).transform.position;
                    break;
                case Token.Left:
                    _direction = UnitManager.Instance.GetRightTurn(_direction);
                    break;
                case Token.Right:
                    _direction = UnitManager.Instance.GetLeftTurn(_direction);
                    break;
                case Token.Capture:
                    _position = _playerController.GetLocalBackwards(_position, _direction).transform.position;
                    break;
                case Token.Shoot:
                    {
                        _hasAlreadyShot = false;
                        //undo shooting animation
                    }                    
                    break;
                case Token.Empty:
                    break;
                default: break;
            }
            setToken(Token.Empty);
            _startTile.SetIgnoreUnit(false);
        }        

        updateRobotGhost();
        updateTokenAvailability();
    }

    /// <summary>
    /// Adds a token to the token list, updating the graphics of the Token Bar.
    /// </summary>
    /// <param name="token">The token to set.</param>
    private void setToken(Token token)
    {
        _tokens[_index] = token;
        Sprite sprite = null;
        Color color = new Color(255, 255, 255, 1f);
        Image image = _inputs[_index].GetComponent<Image>();
        //check to replace Image with Sprite?

        switch (token)
        {
            //maybe add token for stunned?
            case Token.Forward:
                sprite = SpriteManager.Instance.GetForwardTokenSprite();
                break;
            case Token.Backward:
                sprite = SpriteManager.Instance.GetBackwardsTokenSprite();
                break;
            case Token.Left:
                sprite = SpriteManager.Instance.GetLeftTokenSprite();
                break;
            case Token.Right:
                sprite = SpriteManager.Instance.GetRightTokenSprite();
                break;
            case Token.Capture:
                sprite = SpriteManager.Instance.GetCaptureTokenSprite();
                break;
            case Token.Shoot:
                sprite = SpriteManager.Instance.GetShootTokenSprite();
                break;
            case Token.Empty:
                color = new Color(255, 255, 255, 0f);
                break;
            case Token.Stun:
                sprite = SpriteManager.Instance.GetStunSprite();
                break;
            default: break;
        }
        image.sprite = sprite;
        image.color = color;
    }

    /// <summary>
    /// Initialises the state of the token list and the Token Bar.
    /// </summary>
    private void clearTokenSelection()
    {
        for (_index = 0; _index < _tokens.Length; _index++)
        {
            setToken(Token.Empty);
        }
        if(_isStunned)
        {
            for(_index = 2; _index < _tokens.Length; _index++) {
                setToken(Token.Stun);
            }
        }
        _index = 0;
    }
}

