using UnityEngine;
using Mirror;

// This script should be attached to the player GameObject.
// It requires an AudioSource component on the same GameObject.
[RequireComponent(typeof(AudioSource))]
public class PlayerSwordSound : MonoBehaviour
{
    // The Audio Clip to be played when the sword is swung.
    // Assign this in the Unity Inspector.
    public AudioClip swordSwingSound;

    // A reference to the AudioSource component.
    private AudioSource audioSource;

    void Awake()
    {
        // Get the AudioSource component attached to this GameObject.
        audioSource = GetComponent<AudioSource>();

        // Set the AudioSource's clip to the swordSwingSound.
        if (swordSwingSound != null)
        {
            audioSource.clip = swordSwingSound;
        }
    }

    // This public method is called to play the sword swing sound effect.
    public void PlaySwordSwingSound()
    {
        // Check if a sound clip and AudioSource are assigned before playing.
        if (swordSwingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(swordSwingSound);
        }
        else
        {
            Debug.LogWarning("Sword swing sound or AudioSource is missing! Please assign them in the Inspector.");
        }
    }
}
