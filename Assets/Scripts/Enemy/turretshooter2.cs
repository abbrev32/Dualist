using Mirror;
using UnityEngine;

public class turretshooter2 : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public float shootInterval = 3f;
    public float bulletSpeed = 5f;
    public float[] shootAngles = { 150f, 180f, 210f }; // sideway angles

    private float timer;

    void Start()
    {
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