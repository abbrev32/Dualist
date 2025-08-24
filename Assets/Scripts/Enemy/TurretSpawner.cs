using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class TurretSpawner : NetworkBehaviour
{
    public Transform spawnPoint0;
    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public GameObject turretPrefab;
    public GameObject turretPrefab2;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    private void Start()
    {
        if (!isServer) return;
        Spawn();
    }

    [Server]
    void Spawn()
    {
        GameObject turret0 = Instantiate(turretPrefab, spawnPoint0.position, Quaternion.identity);
        GameObject turret1 = Instantiate(turretPrefab, spawnPoint1.position, Quaternion.identity);
        GameObject turret2 = Instantiate(turretPrefab2, spawnPoint2.position, spawnPoint2.rotation);

        NetworkServer.Spawn(turret0);
        NetworkServer.Spawn(turret1);
        NetworkServer.Spawn(turret2);

        // Add them to the list
        spawnedObjects.Add(turret0);
        spawnedObjects.Add(turret1);
        spawnedObjects.Add(turret2);
    }

    void Update()
    {
        if (!isServer) return;

        // Remove destroyed objects
        spawnedObjects.RemoveAll(obj => obj == null);

        //Debug.Log("Remaining objects: " + RemainingCount);
    }

    // Public property so other scripts can read the count
    public int RemainingCount
    {
        get { return spawnedObjects.Count; }
    }
}
