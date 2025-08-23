using Mirror;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TurretShooter3 : NetworkBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public float shootInterval = 3f;
    public float bulletSpeed = 5f;
    public float verticalSpacing = 0.2f; // spacing between bullets vertically
    public AudioClip shootSoundClip;

    private float timer;
    private AudioSource audioSource;

    void Start()
    {
        timer = shootInterval;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isServer) return; // Only run on server

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            ShootTriple();
            timer = shootInterval;
        }
    }

    void ShootTriple()
    {
        if (audioSource != null && shootSoundClip != null)
            audioSource.PlayOneShot(shootSoundClip);

        // Spawn 3 bullets stacked vertically, shooting to the right
        for (int i = -1; i <= 1; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(0, i * verticalSpacing, 0);

            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.linearVelocity = Vector2.right * bulletSpeed;

            // Bullet faces right
            bullet.transform.rotation = Quaternion.Euler(0, 0, 0);

            NetworkServer.Spawn(bullet);
        }
    }
}
