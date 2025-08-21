using UnityEngine;
using Mirror;
using UnityEditor.Experimental.GraphView;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(NetworkIdentity))]
public class Knife : NetworkBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifeTime = 4f;

    private Rigidbody2D rb;

    // Set by the spawner (server)
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
    }

    // Move only on the server; clients get sync via NetworkTransform
    [ServerCallback]
    void FixedUpdate()
    {
        // robust movement without nulls
        rb.linearVelocity = dir * speed;
    }

    [ServerCallback]
    void OnTriggerEnter2D(Collider2D other)
    {
        // OPTIONAL: ignore hitting the owner/player if needed, e.g. if (other.CompareTag("Player")) return;

        if (other.CompareTag("Monster"))
        {
            // Call your monster's Kill()
            var m = other.GetComponent<MonsterMovement>();
            if (m != null) m.Kill();
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