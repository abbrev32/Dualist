using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MonsterSpawner : NetworkBehaviour
{
    [Header("Monster Prefabs")]
    public GameObject monsterPrefab; // OldMonster
    public GameObject ghostPrefab;   // Ghost

    [Header("Spawn Settings")]
    public float spawnDelay = 0.5f;
    public Vector2 speedRange = new Vector2(2f, 5f);
    public float positionOffset = 1.5f;

    [Header("Spawner Health")]
    [SyncVar] public int maxHealth = 3;
    [SyncVar] private int currentHealth;

    private List<GameObject> currentMonsters = new List<GameObject>();

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
        SpawnWave(); // Spawn first wave
    }

    void Update()
    {
        if (!isServer) return;

        // Remove destroyed monsters/ghosts
        currentMonsters.RemoveAll(m => m == null);

        // If all dead, spawn next wave
        if (currentMonsters.Count == 0)
        {
            SpawnWave();
        }
    }

    [Server]
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            DestroyAllMonsters();  // 💥 Kill all monsters when portal dies
            NetworkServer.Destroy(gameObject); // Destroy the portal
        }
    }

    [Server]
    private void SpawnWave()
    {
        StartCoroutine(SpawnMonstersRoutine(6, 4)); // 6 monsters, 4 ghosts
    }

    [Server]
    private IEnumerator SpawnMonstersRoutine(int numMonsters, int numGhosts)
    {
        // Spawn monsters
        for (int i = 0; i < numMonsters; i++)
        {
            Vector3 pos = transform.position;
            pos.y += Random.Range(-positionOffset, positionOffset);

            GameObject newMonster = Instantiate(monsterPrefab, pos, Quaternion.identity);

            MonsterMovement moveScript = newMonster.GetComponent<MonsterMovement>();
            if (moveScript != null)
                moveScript.speed = Random.Range(speedRange.x, speedRange.y);

            NetworkServer.Spawn(newMonster);
            currentMonsters.Add(newMonster);

            yield return new WaitForSeconds(spawnDelay);
        }

        // Spawn ghosts
        for (int i = 0; i < numGhosts; i++)
        {
            Vector3 pos = transform.position;
            pos.y += Random.Range(-positionOffset, positionOffset);

            GameObject newGhost = Instantiate(ghostPrefab, pos, Quaternion.identity);

            NetworkServer.Spawn(newGhost);
            currentMonsters.Add(newGhost);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    [Server]
    private void DestroyAllMonsters()
    {
        foreach (var monster in currentMonsters)
        {
            if (monster != null)
            {
                NetworkServer.Destroy(monster);
            }
        }
        currentMonsters.Clear();
    }
}
