using Mirror;
using UnityEngine;

/// <summary>
/// This script handles a networked turret with predictive aiming and shooting logic.
/// It requires a Rigidbody2D on the player GameObject.
/// </summary>
public class PredictiveTurretShooter : NetworkBehaviour
{
    [Tooltip("Reference to the player's Transform.")]
    public Transform player;

    [Tooltip("Reference to the player's Rigidbody2D.")]
    public Rigidbody2D playerRb;

    public GameObject bulletPrefab;
    public float shootInterval = 3f;
    public float bulletSpeed = 5f;

    private float timer;

    void Start()
    {
        timer = shootInterval;
    }

    void Update()
    {
        // Only the server should run the turret logic.
       // if (!isServer) return;
        if (player == null || playerRb == null) return;

        // Count down the timer.
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            ShootAtTarget();
            timer = shootInterval;
        }
    }

    /// <summary>
    /// Calculates the predicted position and shoots a bullet.
    /// </summary>
    void ShootAtTarget()
    {
        // Calculate the predicted aim position using the player's velocity.
        Vector2 aimPosition = PredictAim(player.position, playerRb.linearVelocity, transform.position, bulletSpeed);

        // Calculate the direction from the turret to the predicted position.
        Vector2 direction = (aimPosition - (Vector2)transform.position).normalized;

        // Create the bullet on the server.
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // Get the bullet's Rigidbody2D and set its velocity.
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = direction * bulletSpeed;
        }

        // Rotate the bullet to face the direction of travel.
        float bulletAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(bulletAngle, Vector3.forward);

        // Spawn the bullet across the network.
        NetworkServer.Spawn(bullet);
    }

    /// <summary>
    /// Predicts the future position of the target.
    /// </summary>
    Vector2 PredictAim(Vector2 targetPos, Vector2 targetVelocity, Vector2 shooterPos, float bulletSpd)
    {
        Vector2 displacement = targetPos - shooterPos;
        float timeToHit = displacement.magnitude / bulletSpd;
        Vector2 predictedPos = targetPos + (targetVelocity * timeToHit);
        return predictedPos;
    }
}