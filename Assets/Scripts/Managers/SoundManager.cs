using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    /*“Sound effects obtained from https://www.zapsplat.com“
    “Additional sound effects from https://www.zapsplat.com“
    “Music from https://www.zapsplat.com“*/

    [SerializeField] private AudioClip _footStepsSound;
    [SerializeField] private AudioClip _shootBeamSound;
    [SerializeField] private AudioClip _shieldUpSound;
    [SerializeField] private AudioClip _shieldDownSound;
    [SerializeField] private AudioClip _captureBuildingSound;
    [SerializeField] private AudioClip _robotStunSound;

    public static SoundManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public AudioClip GetFootStepsSound()
    {
        return _footStepsSound;
    }

    public void PlayShootBeamSound(Vector3 position, float volume = 1f)
    {
        playSoundClientRpc(_shootBeamSound, position, volume);
    }

    public void PlayShieldUpSound(Vector3 position, float volume = 1f)
    {
        playSoundClientRpc(_shieldUpSound, position, volume);
    }

    public void PlayShieldDownSound(Vector3 position, float volume = 1f)
    {
        playSoundClientRpc(_shieldDownSound, position, volume);
    }

    public void PlayCaptureBuildingSound(Vector3 position, float volume = 1f)
    {
        playSoundClientRpc(_captureBuildingSound, position, volume);
    }

    public void PlayRobotStunnedSound(Vector3 position, float volume = 1f)
    {
        playSoundClientRpc(_robotStunSound, position, volume);
    }

    [ClientRpc]
    private void playSoundClientRpc(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }
}
