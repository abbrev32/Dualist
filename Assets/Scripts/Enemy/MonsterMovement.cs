using UnityEngine;
using Mirror;

public class MonsterMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;
    public float minX = -10f, maxX = 10f;
    public float minY = -3f, maxY = 3f;
    private Vector2 direction;

    [Header("Chase Settings")]
    public Transform target;
    [Range(0f, 1f)] public float chaseChancePerSecond = 0.2f;
    public float chaseDuration = 2f;
    public float reachThreshold = 0.5f;

    private bool chasing = false;
    private float chaseTimer = 0f;

    [Header("Randomness")]
    private float lastDirChangeTime = 0f;
    private float dirChangeInterval = 2f; // increase to keep direction longer

    void Start()
    {
        PickRandomDirection();
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;

        // Decide if should start chasing
        if (!chasing && target != null && Random.value < chaseChancePerSecond * deltaTime)
        {
            chasing = true;
            chaseTimer = chaseDuration;
        }

        if (chasing && target != null)
        {
            Vector2 toTarget = ((Vector2)target.position - (Vector2)transform.position);
            if (toTarget.sqrMagnitude > 0.001f)
            {
                direction = toTarget.normalized;
            }

            transform.Translate(direction * speed * deltaTime);

            chaseTimer -= deltaTime;
            if (chaseTimer <= 0f || toTarget.magnitude <= reachThreshold)
            {
                chasing = false;
                PickRandomDirection();
            }
        }
        else
        {
            // Random bee-like movement
            transform.Translate(direction * speed * deltaTime);

            Vector3 pos = transform.position;

            // Smooth bounce at edges
            if (pos.x <= minX) direction.x = Mathf.Abs(direction.x);
            if (pos.x >= maxX) direction.x = -Mathf.Abs(direction.x);
            if (pos.y <= minY) direction.y = Mathf.Abs(direction.y);
            if (pos.y >= maxY) direction.y = -Mathf.Abs(direction.y);

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            transform.position = pos;

            // Occasionally change direction slightly
            if (Time.time - lastDirChangeTime > dirChangeInterval)
            {
                PickRandomDirection();
                lastDirChangeTime = Time.time;
                dirChangeInterval = Random.Range(1.5f, 3f); // keep direction longer for map crossing
            }
        }
    }

    void PickRandomDirection()
    {
        Vector2 r;
        do
        {
            r = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        } while (r.sqrMagnitude < 0.01f);
        direction = r.normalized;

        // Optional: small speed variation
        speed = Random.Range(2.5f, 3.5f);
    }

    [Server]
    public void Kill()
    {
        NetworkServer.Destroy(gameObject);
    }
}