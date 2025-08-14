using Mirror;
using UnityEngine;

public class TurretShooter : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public float shootInterval = 3f;
    public float bulletSpeed = 5f;
    public float[] shootAngles = { 270f, 315f, 225f }; // Downward angles

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