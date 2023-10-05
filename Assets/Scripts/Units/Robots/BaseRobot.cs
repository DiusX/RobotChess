using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

public class BaseRobot : BaseUnit
{
    [SerializeField] private GameObject _childGameObject;
    public NetworkVariable<UnitDirection> direction = new NetworkVariable<UnitDirection>(UnitDirection.South);
    private NetworkVariable<bool> _isStunned = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> _isFlipped = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> _isWalking = new NetworkVariable<bool>(false);
    

    private AudioSource _audioSource;

    private Animator _animator;
    public const string IDLE_SOUTH = "idle s";
    public const string IDLE_WEST = "idle w";
    public const string IDLE_NORTH = "idle n";
    public const string IDLE_EAST = "idle e";
    public const string WALK_SOUTH = "walk s";
    public const string WALK_WEST = "walk w";
    public const string WALK_NORTH = "walk n";
    public const string WALK_EAST = "walk e";
    public const string VICTORY_POSE = "victory";
    public const string DEFEAT_POSE = "defeat";
    
    private string _currentState;    

    public bool IsStunned => _isStunned.Value;

    private void Start()
    {        
        _animator = GetComponent<Animator>();
        _isStunned.OnValueChanged += onStunValueChanged;
        _isFlipped.OnValueChanged += onFlipSpriteValueChanged;
        _isWalking.OnValueChanged += onWalkingValueChanged;

        _audioSource = GetComponent<AudioSource>();
        AudioClip _audioClip = SoundManager.Instance.GetFootStepsSound();
        _audioSource.clip = _audioClip;
    }

    public void FlipSpriteHorizontally(bool newValue)
    {
        if (newValue != _isFlipped.Value)
        {
            _isFlipped.Value = newValue;
            _isFlipped.SetDirty(true);
        }
    }
   
    public void SetWalking(bool newValue)
    {
        _isWalking.Value = newValue;
    }

    private void onStunValueChanged(bool oldValue, bool newValue)
    {
        _childGameObject.SetActive(newValue);
        onClientRpc(newValue);
    }

    [ClientRpc]
    private void onClientRpc(bool newValue)
    {
        _childGameObject.SetActive(newValue);
    }

    private void onFlipSpriteValueChanged(bool oldValue, bool newValue)
    {
        GetComponent<SpriteRenderer>().flipX = newValue;
    }

    private void onWalkingValueChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            _audioSource.Play();
        }
        else
        {
            _audioSource.Stop();
        }   
    }

    public void ChangeAnimationState(string newState)
    {
        //stop the same animation from self-interruption
        if (_currentState == newState) return;

        //play the animation
        _animator.Play(newState);

        //reassign the current state
        _currentState = newState;
    }

    public override void GetShot(Faction faction)
    {
        if(Faction != faction)
        {
            _isStunned.Value = true;
            _isStunned.SetDirty(true);
            SoundManager.Instance.PlayRobotStunnedSound(transform.position);
            AnimationManager.Instance.PlayParticleClientRpc(transform.position);
        }
    }

    /*[ClientRpc]
    public override void GetShotClientRpc(Faction faction)
    {
        GetShot(faction);
    }*/

    public override void ClearShot()
    {
        _isStunned.Value = false;
        _isStunned.SetDirty(true);
    }

    /*[ClientRpc]
    public override void ClearShotClientRpc()
    {
        ClearShot();
    }*/

   /* [ClientRpc]
    public void TurnLeftClientRpc()
    {
        direction = UnitManager.Instance.GetLeftTurn(direction);
    }

    [ClientRpc]
    public void TurnRightClientRpc()
    {
        direction = UnitManager.Instance.GetRightTurn(direction);
    }
*/
}
