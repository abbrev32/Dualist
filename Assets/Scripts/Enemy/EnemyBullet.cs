using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    public float force;

    void Start()
    {
       rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        Vector3 direction = player.transform.position - transform.position;
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;

        float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>().TakeDamage(1);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Border"))
        {
            Destroy(gameObject);
        }
    }

}