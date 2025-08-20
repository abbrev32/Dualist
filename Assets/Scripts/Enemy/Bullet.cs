using Mirror;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public PlayerFaction.Faction canDamageTo;
    public SpriteRenderer sprite;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!NetworkServer.active) return;

        if (collision.collider.CompareTag("Player"))
        {
            //Visibility&Damage link
            var localPlayer = NetworkClient.localPlayer?.GetComponent<PlayerFaction>();
            if (localPlayer.faction != canDamageTo) return;

            Debug.Log("Player Hit!");
            PlayerHealth playerHealth = collision.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
                Kill();
            }
        }

        if (collision.collider.CompareTag("Border"))
        {
            Debug.Log("Wall Hit!");
            Kill();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!NetworkServer.active) return;

        if (collision.CompareTag("Sword"))
        {
            Debug.Log("Sword Blocked!");
            Kill();
        }
    }

    //make bullet also invisible
    private void Start()
    {
        if (isClient)
        {
            UpdateVisibility();
        }
    }

    void UpdateVisibility()
    {
        var localPlayer = NetworkClient.localPlayer?.GetComponent<PlayerFaction>();
        if (localPlayer == null) return;

        bool visible = (localPlayer.faction == canDamageTo);
        sprite.enabled = visible;
    }



    [Server]
    void Kill()
    {
        NetworkServer.Destroy(gameObject); // destroys across all clients
    }
}
