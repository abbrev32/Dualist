using Mirror;
using UnityEngine;

/// <summary>
/// This script handles the turret's shooting logic and plays a sound directly.
/// It requires an AudioSource component on the same GameObject.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class TurretShooter : NetworkBehaviour
{
    // A reference to the AudioSource component that will play the sound.
    private AudioSource audioSource;

    [Tooltip("The audio clip to play when the turret fires.")]
    public AudioClip shootSoundClip;

    public GameObject bulletPrefab;
    public float shootInterval = 3f;
    public float bulletSpeed = 5f;
    public float[] shootAngles = { 270f, 315f, 225f }; // Downward angles

    private float timer;

    void Start()
    {
        // Get the AudioSource component on the same GameObject.
        audioSource = GetComponent<AudioSource>();

        timer = shootInterval;
    }

    void Update()
    {
        if (!isServer) return;
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            ShootInAngles();
            timer = shootInterval;
        }
    }

    void ShootInAngles()
    {
        // Play the sound directly before creating the bullets.
        if (shootSoundClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSoundClip);
        }
        else
        {
            // A warning to remind the user to assign the clip in the Inspector.
            Debug.LogWarning("No shoot sound clip or AudioSource found!");
        }

        foreach (float angle in shootAngles)
        {
            // Create bullet
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            // Calculate direction based on angle
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;

            // Set bullet velocity - FIXED: Using velocity instead of linearVelocity
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.linearVelocity = direction * bulletSpeed;

            // Rotate bullet to face direction
            float bulletAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.AngleAxis(bulletAngle, Vector3.forward);

            NetworkServer.Spawn(bullet);
        }
    }
}