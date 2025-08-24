using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float force = 10f;
    public int damage = 1;
    public float yOffset = 0.5f; // Adjust this value to aim higher

    private Rigidbody2D rb;
    private Transform target;

    // Public method to receive the target from the turret
    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
        InitializeMovement();
    }

    void InitializeMovement()
    {
        rb = GetComponent<Rigidbody2D>();

        if (target != null)
        {
            // Add y-axis offset to target position
            Vector3 adjustedTargetPos = target.position + new Vector3(0, yOffset, 0);

            Vector3 direction = (adjustedTargetPos - transform.position).normalized;
            rb.linearVelocity = direction * force;

            // Calculate rotation to point toward adjusted target
            float rot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rot);
        }
        else
        {
            // If no target, destroy the bullet
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Border"))
        {
            Destroy(gameObject);
        }
    }
}
