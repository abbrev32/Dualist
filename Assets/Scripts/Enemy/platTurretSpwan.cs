using Mirror;
using UnityEngine;

public class PlatTurretSpawn : NetworkBehaviour
{
    [Header("Turret Settings")]
    public GameObject turretPrefab1;   // First turret prefab
    public GameObject turretPrefab2;   // Second turret prefab

    public Transform spawnPoint1;      // Position for first turret
    public Transform spawnPoint2;      // Position for second turret

    private bool hasSpawned = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer) return; // Only server spawns turrets

        if (!hasSpawned && collision.CompareTag("Player"))
        {
            // Spawn first turret
            if (turretPrefab1 != null && spawnPoint1 != null)
            {
                GameObject turret1 = Instantiate(turretPrefab1, spawnPoint1.position, Quaternion.identity);
                NetworkServer.Spawn(turret1);
            }

            // Spawn second turret
            if (turretPrefab2 != null && spawnPoint2 != null)
            {
                GameObject turret2 = Instantiate(turretPrefab2, spawnPoint2.position, Quaternion.identity);
                NetworkServer.Spawn(turret2);
            }

            hasSpawned = true; // Prevent multiple spawns
        }
    }
}
