using Mirror;
using UnityEngine;

public class MonsterSpawner : NetworkBehaviour
{
    public GameObject monsterPrefab; // Assign your Monster prefab in Inspector
    public float spawnInterval = 3f; // Time between spawns

    public override void OnStartServer()
    {
        SpawnMonster();
    }

    [Server]
    public void SpawnMonster()
    {
        GameObject monster = Instantiate(monsterPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(monster);
        Invoke(nameof(SpawnMonster), spawnInterval);
    }
}