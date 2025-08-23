using Mirror;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TurretShooter4 : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public float shootInterval = 3f;
    public float bulletSpeed = 5f;
    public float verticalSpacing = 0.2f; // horizontal spacing between bullets
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
            ShootTripleDown();
            timer = shootInterval;
        }
    }

    void ShootTripleDown()
    {
        if (audioSource != null && shootSoundClip != null)
            audioSource.PlayOneShot(shootSoundClip);

        // Spawn 3 bullets in a horizontal spread while moving downward
        for (int i = -1; i <= 1; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(i * verticalSpacing, 0, 0);

            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.linearVelocity = Vector2.down * bulletSpeed; // move downward

            // Rotate bullet to face downward
            bullet.transform.rotation = Quaternion.Euler(0, 0, -90f);

            NetworkServer.Spawn(bullet);
        }
    }
}
