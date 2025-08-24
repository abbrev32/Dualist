using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(NetworkIdentity))]
public class Knife : NetworkBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifeTime = 4f;
    [SerializeField] private int damage = 1;

    private Rigidbody2D rb;
    private Collider2D col;

    [SyncVar] private Vector2 dir = Vector2.right;

    [Server]
    public void Initialize(Vector2 direction)
    {
        dir = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector2.right;
        Invoke(nameof(Despawn), lifeTime);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        if (col != null) col.isTrigger = true;
    }

    [ServerCallback]
    void FixedUpdate()
    {
        rb.linearVelocity = dir * speed;
    }

    [ServerCallback]
    void OnTriggerEnter2D(Collider2D other) => HandleHit(other);

    [ServerCallback]
    void OnCollisionEnter2D(Collision2D collision) => HandleHit(collision.collider);

    [Server]
    void HandleHit(Collider2D other)
    {
        if (other.CompareTag("Player")) return;

        // --- MONSTER ---
        if (other.CompareTag("Monster"))
        {
            if (other.TryGetComponent(out Monster mA)) { mA.Kill(); Despawn(); return; }
            if (other.TryGetComponent(out MonsterMovement mB)) { mB.Kill(); Despawn(); return; }

            var mAp = other.GetComponentInParent<Monster>();
            if (mAp != null) { mAp.Kill(); Despawn(); return; }
            var mBp = other.GetComponentInParent<MonsterMovement>();
            if (mBp != null) { mBp.Kill(); Despawn(); return; }
        }

        // --- GHOST DARK ---
        if (other.TryGetComponent(out GhostDark ghost))
        {
            NetworkServer.Destroy(ghost.gameObject);
            Despawn();
            return;
        }

        // --- TURRET ---
        if (other.CompareTag("Turret"))
        {
            if (other.TryGetComponent(out TurretHealth th)) { th.TakeDamage(damage); Despawn(); return; }
            var thp = other.GetComponentInParent<TurretHealth>();
            if (thp != null) { thp.TakeDamage(damage); Despawn(); return; }
        }

        // --- MONSTER SPAWNER ---
        if (other.CompareTag("MonsterSpawner"))
        {
            if (other.TryGetComponent(out MonsterSpawner sp)) { sp.TakeDamage(damage); Despawn(); return; }
            var spp = other.GetComponentInParent<MonsterSpawner>();
            if (spp != null) { spp.TakeDamage(damage); Despawn(); return; }
        }

        Despawn();
    }

    [Server]
    void Despawn()
    {
        if (gameObject != null)
            NetworkServer.Destroy(gameObject);
    }
}
