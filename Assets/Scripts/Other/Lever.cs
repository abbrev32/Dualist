using UnityEngine;
using Mirror;

public class Lever : NetworkBehaviour
{
    public Elevator elevator; // assign in Inspector

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (elevator == null) return;

        if (other.CompareTag("Player") || other.CompareTag("Monster"))
        {
            // Any player or monster toggles the elevator
            elevator.ToggleElevator();
        }
        
    }
}