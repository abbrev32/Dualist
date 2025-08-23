using Mirror;
using UnityEngine;

/// <summary>
/// This script manages a turret's shooting behavior in a networked environment.
/// It fires bullets at specific angles at a set interval.
/// It now handles its own audio playback.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class turretshooter2 : NetworkBehaviour
{
    // Public variables for customization in the Unity Editor
    public GameObject bulletPrefab;
    public float shootInterval = 3f;
    public float bulletSpeed = 5f;
    public float[] shootAngles = { 150f, 180f, 210f }; // sideway angles

    [Tooltip("The audio clip to play when the turret fires.")]
    public AudioClip shootSoundClip;

    // Private variables for internal logic
    private float timer;
    private AudioSource audioSource;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Used to initialize the timer and get a reference to the AudioSource component.
    /// </summary>
    void Start()
    {
        // Initialize the timer to the shoot interval
        timer = shootInterval;
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// This method is called once per frame.
    /// It handles the shooting logic on the server.
    /// </summary>
    void Update()
    {
        // Only run the shooting logic on the server to prevent duplicates
        if (!isServer) return;

        // Decrease the timer by the time elapsed since the last frame
        timer -= Time.deltaTime;

        // Check if the timer has run out
        if (timer <= 0)
        {
            // Call the method to handle shooting
            ShootInAngles();
            // Reset the timer
            timer = shootInterval;
        }
    }

    /// <summary>
    /// Handles the creation and spawning of bullets for each defined angle.
    /// </summary>
    void ShootInAngles()
    {
        // Play the sound before shooting the bullets, but only if the components are available
        if (audioSource != null && shootSoundClip != null)
        {
            audioSource.PlayOneShot(shootSoundClip);
        }

        // Loop through each angle in the array
        foreach (float angle in shootAngles)
        {
            // Create a bullet at the turret's position with no initial rotation
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            // Calculate the direction vector based on the angle
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;

            // Get the Rigidbody2D component of the bullet
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

            // Set the bullet's velocity in the calculated direction
            bulletRb.linearVelocity = direction * bulletSpeed;

            // Rotate the bullet sprite to face its direction of movement
            float bulletAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.AngleAxis(bulletAngle, Vector3.forward);

            // Spawn the bullet on the network for all clients
            NetworkServer.Spawn(bullet);
        }
    }
}
