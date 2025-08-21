using UnityEngine;

/// <summary>
/// This script handles the sound for a turret's firing action.
/// It requires an AudioSource component on the same GameObject.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class TurretSound : MonoBehaviour
{
    // A reference to the AudioSource component that will play the sound.
    // [HideInInspector] is used here because the script will find the component automatically.
    private AudioSource audioSource;

    [Tooltip("The audio clip to play when the turret fires.")]
    public AudioClip shootSoundClip;

    [Tooltip("The volume at which to play the sound.")]
    [Range(0.0f, 1.0f)]
    public float volume = 1.0f;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// We get a reference to the AudioSource component here.
    /// </summary>
    void Awake()
    {
        // Get the AudioSource component attached to this GameObject.
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Plays the configured shoot sound clip.
    /// This method should be called from your turret's firing logic.
    /// </summary>
    public void PlayShootSound()
    {
        // Check if a sound clip has been assigned.
        if (shootSoundClip != null)
        {
            // Play the sound once at the specified volume.
            audioSource.PlayOneShot(shootSoundClip, volume);
        }
        else
        {
            // Log a warning if no sound is assigned to help with debugging.
            Debug.LogWarning("No shoot sound clip assigned to the turret sound script!");
        }
    }
}

