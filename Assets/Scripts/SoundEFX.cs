using UnityEngine;

public class SoundEFX : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource SFXSource;

    [Header("Audio Clip")]
    public AudioClip jump;
    public AudioClip sword_waving;

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

}
