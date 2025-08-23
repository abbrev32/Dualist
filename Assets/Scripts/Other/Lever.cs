using UnityEngine;
using Mirror;

public class Lever : NetworkBehaviour
{
    public Elevator elevator; // assign in Inspector

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (elevator == null) return;

        if (other.CompareTag("Player"))
        {
            // Player can only move elevator down
            elevator.MoveToBottom();

            // Enable collider again
            PolygonCollider2D col = elevator.GetComponent<PolygonCollider2D>();
            if (col != null)
            {
                col.enabled = true;
            }
        }
        else if (other.CompareTag("Monster"))
        {
            // Monster can only move elevator up
            elevator.MoveToTop();

            // Disable elevator collider
            PolygonCollider2D col = elevator.GetComponent<PolygonCollider2D>();
            if (col != null)
            {
                col.enabled = false;
            }
        }
    }
}
