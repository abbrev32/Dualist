using UnityEngine;

/// <summary>
/// A simple script to handle playing a sound effect for a dash action.
/// Attach this script to the GameObject that will be making the sound.
/// </summary>
public class DashSoundPlayer : MonoBehaviour
{
    // Public variables allow us to assign these in the Unity Inspector.
    // The AudioSource component that will play the sound.
    public AudioSource audioSource;
    // The sound clip to be played when the dash occurs.
    public AudioClip dashSound;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// It's a good place to set up references.
    /// </summary>
    private void Awake()
    {
        // Check if the AudioSource is not assigned. If it's not, try to get it from the current GameObject.
        // This makes the script more flexible, allowing you to either assign it in the Inspector or have it automatically
        // find a component on the same object.
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    /// <summary>
    /// This method plays the dash sound. You can call this from your player's
    /// movement script when the dash action is triggered.
    /// </summary>
    public void PlayDashSound()
    {
        // Check if both the audioSource and the dashSound clip are valid.
        // This prevents errors if they haven't been assigned correctly.
        if (audioSource != null && dashSound != null)
        {
            // Use PlayOneShot to avoid interrupting any other sounds that might be playing.
            // PlayOneShot is ideal for quick, one-off sound effects like this.
            audioSource.PlayOneShot(dashSound);
        }
        else
        {
            // Log a warning in the console to help with debugging if something is missing.
            Debug.LogWarning("AudioSource or Dash Sound clip not assigned on the DashSoundPlayer script!");
        }
    }
}
