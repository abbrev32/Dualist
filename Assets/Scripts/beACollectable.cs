using JetBrains.Annotations;
using Mirror;
using System.Globalization;
using UnityEngine;

public class beACollectable : NetworkBehaviour
{
    public GameObject collectiblePrefab;
    public Transform collectibleSpawnPoint;
    public MonsterSpawner monsterSpawner;

    [Server]
    public void TrySpawnCollectable()
    {
        GameObject collectible = Instantiate(collectiblePrefab, collectibleSpawnPoint.position, Quaternion.identity);
        NetworkServer.Spawn(collectible);
    }
}