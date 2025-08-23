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
            // Player moves elevator DOWN
            elevator.MoveToBottom();
        }
        else if (other.CompareTag("Monster"))
        {
            // Monster moves elevator UP
            elevator.MoveToTop();
        }
    }
}
