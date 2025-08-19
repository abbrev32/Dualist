using Mirror;
using UnityEngine;

public class TurretSpawner : NetworkBehaviour
{
    public Transform spawnPoint;
    public GameObject turretPrefab;
    private void Start()
    {
        if (!isServer) return;
        Spawn();
    }
    [Server]
    void Spawn()
    {
        GameObject turret = Instantiate(turretPrefab, spawnPoint.position, Quaternion.identity);
        NetworkServer.Spawn(turret);
    }
}
