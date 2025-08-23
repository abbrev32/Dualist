using Mirror;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TurretShooter4 : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public float shootInterval = 3f;
    public float bulletSpeed = 5f;
    public float verticalSpacing = 0.2f; // vertical spread
    public AudioClip shootSoundClip;

    private float timer;
    private AudioSource audioSource;
    [SyncVar] private bool isShooting = false;

    void Start()
    {
        timer = shootInterval;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isServer) return;
        if (!isShooting) return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ShootTripleRight();
            timer = shootInterval;
        }
    }

    void ShootTripleRight()
    {
        if (audioSource != null && shootSoundClip != null)
            audioSource.PlayOneShot(shootSoundClip);

        for (int i = -1; i <= 1; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(0, i * verticalSpacing, 0);
            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector2.right * bulletSpeed;
            NetworkServer.Spawn(bullet);
        }
    }

    [Server]
    public void StartShooting() => isShooting = true;

    [Server]
    public void StopShooting() => isShooting = false;
}
