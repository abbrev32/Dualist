using UnityEngine;

/// <summary>
/// This script controls the volume of a turret's sound based on the distance to the player.
/// Attach this script to your turret GameObject.
/// </summary>
public class TurretSoundController : MonoBehaviour
{
    // A reference to the player's transform. You can set this in the Inspector.
    public Transform playerTransform;

    // The AudioSource component on the turret.
    private AudioSource audioSource;

    // The distance at which the sound starts to fade.
    public float maxDistance = 10f;
    // The distance at which the sound is at its maximum volume.
    public float minDistance = 2f;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// It's a good place to set up references.
    /// </summary>
    private void Awake()
    {
        // Get the AudioSource component on this GameObject.
        audioSource = GetComponent<AudioSource>();

        // If there's no AudioSource, add one to the GameObject.
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Set the sound to loop so it plays continuously.
        audioSource.loop = true;

        // If the audio source is not playing, start it.
        // This makes sure the sound is always on to be faded.
        if (!audioSource.isPlaying && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    /// <summary>
    /// Update is called once per frame. We'll use this to continuously
    /// check the distance to the player and adjust the volume.
    /// </summary>
    private void Update()
    {
        // Check if the player's transform is assigned.
        if (playerTransform == null)
        {
            Debug.LogWarning("Player Transform not assigned on the TurretSoundController script!");
            return;
        }

        // Calculate the distance between the turret and the player.
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // Map the distance to a volume level.
        // We use Mathf.InverseLerp to get a value between 0 and 1.
        // If distance is less than or equal to minDistance, volume is 1.
        // If distance is greater than or equal to maxDistance, volume is 0.
        // For distances in between, it will be a value from 0 to 1.
        float volume = Mathf.InverseLerp(maxDistance, minDistance, distance);

        // Clamp the volume to ensure it stays between 0 and 1.
        audioSource.volume = Mathf.Clamp01(volume);
    }
}

