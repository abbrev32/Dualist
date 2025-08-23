using UnityEngine;

/// <summary>
/// This script handles the background music for the game.
/// It requires an AudioSource component on the same GameObject.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    [Tooltip("The audio clip to play as background music.")]
    public AudioClip backgroundClip;

    [Tooltip("The volume for the background music.")]
    [Range(0.0f, 1.0f)]
    public float volume = 0.5f;

    private AudioSource audioSource;

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
    /// Called once at the beginning of the game.
    /// We set up and play the background music here.
    /// </summary>
    void Start()
    {
        // Check if a background clip has been assigned.
        if (backgroundClip != null)
        {
            audioSource.clip = backgroundClip;
            audioSource.volume = volume;
            audioSource.loop = true; // Make sure the music loops continuously.
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No background audio clip assigned!");
        }
        /// <summary>
        /// A public method to stop the background music.
        /// </summary>
    
    }
    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}

