using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class InputController : MonoBehaviour
{
    public static InputController Instance;
    [SerializeField] private GameObject _inputOne, _inputTwo, _inputThree, _inputFour;
    [SerializeField] private PlayerController _playerController;    

    private InputToken _inputToken;

    public void Awake()
    {
        Instance = this;
        _inputToken = new InputToken(_inputOne, _inputTwo, _inputThree, _inputFour, _playerController);
    }

    public void InitTempRobot(Tile position, BaseRobot.Direction direction)
    {
        _inputToken.InitTempRobot(position, direction);
    }

    public void MoveForwardToken()
    {
        _inputToken.AddToken(Token.Forward);
    }

    public void MoveBackwardsToken()
    {
        _inputToken.AddToken(Token.Backward);
    }

    public void TurnRightToken()
    {
        _inputToken.AddToken(Token.Right);
    }

    public void TurnLeftToken()
    {
        _inputToken.AddToken(Token.Left);
    }

    public void CaptureToken()
    {
        _inputToken.AddToken(Token.Capture);
    }

    public void CommitToken()
    {
        _inputToken.CommitMoves();
    }

    public void UndoToken()
    {
        _inputToken.UndoMove();
    }
}

enum Token
{
    Forward = 0,
    Backward = 1,
    Left = 2,
    Right = 3,
    Capture = 4,
    Empty = -1
}

internal class InputToken
{
    private Token[] _tokens;
    private GameObject[] _inputs;
    private int _index;

    private Tile position;
    private BaseRobot.Direction direction;
    private readonly PlayerController _playerController;

    public InputToken(GameObject inputOne, GameObject inputTwo, GameObject inputThree, GameObject inputFour, PlayerController _playerController) {
        _tokens = new Token[4];
        _inputs = new GameObject[4];
        _inputs[0] = inputOne;
        _inputs[1] = inputTwo;
        _inputs[2] = inputThree;
        _inputs[3] = inputFour;
        this._playerController = _playerController;
    }

    public void InitTempRobot(Tile position, BaseRobot.Direction direction)
    {
        this.position = position;
        this.direction = direction;
        clearTokenSelection();
        position.setIgnoreUnit(true); //This is to prevent unit from seeing itself in the movement calculations.
    }

    public void AddToken(Token token)
    {
        string message;
        if (_index >= _inputs.Length)
        {
            //MenuManager notify: Max 4 moves.
            message = "Max 4 tokens allowed. Try to Commit";
            MenuManager.Instance.ShowInfoPopup(message);
            return;
        }
        switch (token){
            case (Token.Forward):
                {
                    if (_index > 0 && _tokens[_index - 1] == Token.Backward)
                    {
                        //MenuManager notify: Can't cancel out moves
                        message = "Can not cancel out moves. Try to Undo";
                        MenuManager.Instance.ShowInfoPopup(message);
                        break;
                    }
                    if (_index > 2 && _tokens[_index - 1] == Token.Forward && _tokens[_index - 2] == Token.Forward) {
                        //MenuManager notify: Can't have more than 2 of same tokens after each other
                        message = "Can not use token more than 2 times in a row. Try turning.";
                        MenuManager.Instance.ShowInfoPopup(message);
                        break;
                    }
                    //implement movement
                    Tile forward = _playerController.GetForward(position, direction, out message);
                    if (forward == null)
                    {
                        //MenuManger notify: Unable to move forward
                        if(message == null)
                        {
                            message = "There is something preventing forward movement at this point.";
                        }                        
                        MenuManager.Instance.ShowInfoPopup(message);
                        return;
                    }
                    position = forward;
                    setToken(Token.Forward);
                    _index++;
                    break;
                }
            case(Token.Backward):
                {
                    if (_index > 0 && _tokens[_index - 1] == Token.Forward)
                    {
                        //MenuManager notify: Can't cancel out moves
                        message = "Can not cancel out moves. Try to Undo";
                        MenuManager.Instance.ShowInfoPopup(message);
                        break;
                    }
                    if (_index > 2 && _tokens[_index - 1] == Token.Backward && _tokens[_index - 2] == Token.Backward)
                    {
                        //MenuManager notify: Can't have more than 2 of same tokens after each other
                        message = "Can not use token more than 2 times in a row. Try turning.";
                        MenuManager.Instance.ShowInfoPopup(message);
                        break;
                    }
                    //implement movement
                    Tile backwards = _playerController.GetBackwards(position, direction, out message);
                    if (backwards == null)
                    {
                        //MenuManger notify: Unable to move backwards
                        if(message == null)
                        {
                            message = "There is something preventing backwards movement at this point.";
                        }                        
                        MenuManager.Instance.ShowInfoPopup(message);
                        return;
                    }
                    position = backwards;
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
                        MenuManager.Instance.ShowInfoPopup(message);
                        break;
                    }
                    if (_index > 2 && _tokens[_index - 1] == Token.Left && _tokens[_index - 2] == Token.Left)
                    {
                        //MenuManager notify: Can't have more than 2 of same tokens after each other
                        message = "Can not use token more than 2 times in a row. Try moving.";
                        MenuManager.Instance.ShowInfoPopup(message);
                        break;
                    }
                    //implement movement
                    direction = _playerController.GetLeftTurn(direction);
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
                        MenuManager.Instance.ShowInfoPopup(message);
                        break;
                    }
                    if (_index > 2 && _tokens[_index - 1] == Token.Right && _tokens[_index - 2] == Token.Right)
                    {
                        //MenuManager notify: Can't have more than 2 of same tokens after each other
                        message = "Can not use token more than 2 times in a row. Try moving.";
                        MenuManager.Instance.ShowInfoPopup(message);
                        break;
                    }
                    //implement movement
                    direction = _playerController.GetRightTurn(direction);
                    setToken(Token.Right);
                    _index++;
                    break;
                }
            case (Token.Capture):
                {
                    if (_index != 3)
                    {
                        //MenuManager notify: Capture has to be on last move
                        message = "Capture has to be on the last move.";
                        MenuManager.Instance.ShowInfoPopup(message);
                        break;
                    }
                    //implement movement
                    if (!_playerController.GetCapture(position, direction, GameManager.Instance.Gamestate == GameState.PlayerTurn ? Faction.Player : Faction.Enemy, out message))
                    {
                        //MenuManager notify: Can't capture
                        if(message == null)
                        {
                            message = "Can not perform capture here.";
                        }                        
                        MenuManager.Instance.ShowInfoPopup(message);
                        return;
                    }
                    setToken(Token.Capture);
                    _index++;
                    break;
                }
            default: break;
        }
        //TODO: Check if _index == 4,  then highlight Commit token, else disable highlight
    }

    public void CommitMoves()
    {
        //implement Commit
        if( _index != 4 ) {
            //MenuManager notify: 4 moves required
            string message = "4 tokens are required to be able to commit.";
            MenuManager.Instance.ShowInfoPopup(message);
            return;
        }
        position.setIgnoreUnit(false);
        bool playerTurn = GameManager.Instance.Gamestate == GameState.PlayerTurn;
        for (int i = 0; i < _index; i++)
        {
            if (playerTurn)
            {
                switch (_tokens[i])
                {
                    case Token.Forward:
                        _playerController.MovePlayerForward();
                        break;
                    case Token.Backward:
                        _playerController.MovePlayerBackwards();
                        break;
                    case Token.Left:
                        _playerController.TurnPlayerLeft();
                        break;
                    case Token.Right:
                        _playerController.TurnPlayerRight();
                        break;
                    case Token.Capture:
                        _playerController.PlayerCapture();
                        break;
                    case Token.Empty:
                        break;
                    default: break;
                }
            }
            else
            {
                switch (_tokens[i])
                {
                    case Token.Forward:
                        _playerController.MoveEnemyForward();
                        break;
                    case Token.Backward:
                        _playerController.MoveEnemyBackwards();
                        break;
                    case Token.Left:
                        _playerController.TurnEnemyLeft();
                        break;
                    case Token.Right:
                        _playerController.TurnEnemyRight();
                        break;
                    case Token.Capture:
                        _playerController.EnemyCapture();
                        break;
                    case Token.Empty:
                        break;
                    default: break;
                }
            }
        }
        if (GameManager.Instance.Gamestate == GameState.PlayerTurn)
        {
            GameManager.Instance.ChangeState(GameState.EnemyTurn);
        }
        else
        {
            GameManager.Instance.ChangeState(GameState.PlayerTurn);
        }
    }
    public void UndoMove()
    {
        if (_index > 0)
        {
            //undo movement
            switch (_tokens[--_index])
            {
                case Token.Forward:
                    position = _playerController.GetBackwards(position, direction, out string messageF);
                    break;
                case Token.Backward:
                    position = _playerController.GetForward(position, direction, out string messageB);
                    break;
                case Token.Left:
                    direction = _playerController.GetRightTurn(direction);
                    break;
                case Token.Right:
                    direction = _playerController.GetLeftTurn(direction);
                    break;
                case Token.Capture:
                    break;
                case Token.Empty:
                    break;
                default: break;
            }
            setToken(Token.Empty);
        }        
    }

    private void setToken(Token token)
    {
        _tokens[_index] = token;
        Sprite sprite = null;
        Color color = new Color(255, 255, 255, 1f);
        Image image = _inputs[_index].GetComponent<Image>();

        switch (token)
        {           
            case Token.Forward: sprite = SpriteManager.Instance.GetForwardTokenSprite();
                break; 
            case Token.Backward: sprite = SpriteManager.Instance.GetBackwardsTokenSprite();
                break; 
            case Token.Left: sprite = SpriteManager.Instance.GetLeftTokenSprite();
                break; 
            case Token.Right: sprite = SpriteManager.Instance.GetRightTokenSprite();
                break;
            case Token.Capture: sprite = SpriteManager.Instance.GetCaptureTokenSprite();
                break;
            case Token.Empty: sprite = null;
                color = new Color(255, 255, 255, 0f);
                break;
            default: break;
        }
        image.sprite = sprite;
        image.color = color;
        //TODO: Null does not change image. Fix this (maybe with different image that is blank-ish)
    }

    private void clearTokenSelection()
    {
        for(_index = 0; _index < _tokens.Length; _index++)
        {
            setToken(Token.Empty);
        }
        _index = 0;      
    }
}