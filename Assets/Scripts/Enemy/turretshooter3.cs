using Mirror;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class turretshooter3 : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public float shootInterval = 3f;
    public float bulletSpeed = 5f;
    public float verticalSpacing = 0.2f;
    public AudioClip shootSoundClip;

    private float timer;
    private AudioSource audioSource;

    // Track whether the player is inside the detection zone
    private bool playerInRange = false;

    void Start()
    {
        timer = shootInterval;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isServer) return; // Only on server

        // Only shoot if player is in range
        if (playerInRange)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                ShootTriple();
                timer = shootInterval;
            }
        }
    }

    void ShootTriple()
    {
        if (audioSource != null && shootSoundClip != null)
            audioSource.PlayOneShot(shootSoundClip);

        for (int i = -1; i <= 1; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(0, i * verticalSpacing, 0);
            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.linearVelocity = Vector2.right * bulletSpeed;

            bullet.transform.rotation = Quaternion.Euler(0, 0, 0);
            NetworkServer.Spawn(bullet);
        }
    }

    // Detect when the player enters
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Make sure player GameObject has tag "Player"
        {
            playerInRange = true;
        }
    }

    // Detect when the player leaves
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
