using Mirror;
using UnityEngine;

public class platTurretSpwan : NetworkBehaviour
{
    [Header("Downward Shooting Turrets")]
    public TurretShooter3[] downwardTurrets;

    [Header("Right Shooting Turrets")]
    public TurretShooter4[] rightTurrets;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isServer) return;

        if (other.CompareTag("Player"))
        {
            foreach (var t in downwardTurrets) t.StartShooting();
            foreach (var t in rightTurrets) t.StartShooting();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isServer) return;

        if (other.CompareTag("Player"))
        {
            foreach (var t in downwardTurrets) t.StopShooting();
            foreach (var t in rightTurrets) t.StopShooting();
        }
    }
}
