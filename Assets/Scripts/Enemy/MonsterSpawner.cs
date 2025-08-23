using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MonsterSpawner : NetworkBehaviour
{
    [Header("Prefabs (assign in Inspector)")]
    public GameObject monsterPrefab; // normal monster
    public GameObject ghostPrefab;   // ghost

    [Header("Counts per wave")]
    public int monstersPerWave = 6;
    public int ghostsPerWave = 4;

    [Header("Delays")]
    public float monsterSpawnDelay = 0.5f; // between monsters
    public float ghostSpawnDelay   = 3f;   // between ghosts

    [Header("Spawn variation")]
    public Vector2 monsterSpeedRange = new Vector2(2f, 5f);
    public float positionOffsetY = 1.5f;

    [Header("Spawner Health")]
    [SyncVar] public int maxHealth = 3;
    [SyncVar] private int currentHealth;

    private readonly List<GameObject> currentMonsters = new List<GameObject>();

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
        SpawnWave();
    }

    void Update()
    {
        if (!isServer) return;

        // cleanup
        currentMonsters.RemoveAll(m => m == null);

        // auto next wave when all dead
        if (currentMonsters.Count == 0)
            SpawnWave();
    }

    [Server]
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            // despawn all from this portal
            foreach (var m in currentMonsters)
                if (m != null) NetworkServer.Destroy(m);

            currentMonsters.Clear();
            NetworkServer.Destroy(gameObject); // destroy portal
        }
    }

    [Server]
    void SpawnWave()
    {
        StartCoroutine(SpawnWaveRoutine());
    }

    [Server]
    IEnumerator SpawnWaveRoutine()
    {
        // spawn monsters first (short delay)
        for (int i = 0; i < monstersPerWave; i++)
        {
            SpawnOne(monsterPrefab, trySetMonsterSpeed:true);
            yield return new WaitForSeconds(monsterSpawnDelay);
        }

        // then ghosts (3s delay each)
        for (int i = 0; i < ghostsPerWave; i++)
        {
            SpawnOne(ghostPrefab, trySetMonsterSpeed:false); // ghosts handle their own movement
            yield return new WaitForSeconds(ghostSpawnDelay);
        }
    }

    [Server]
    void SpawnOne(GameObject prefab, bool trySetMonsterSpeed)
    {
        Vector3 pos = transform.position;
        pos.y += Random.Range(-positionOffsetY, positionOffsetY);

        GameObject go = Instantiate(prefab, pos, Quaternion.identity);

        // only set speed on normal monsters that have MonsterMovement
        if (trySetMonsterSpeed && go.TryGetComponent<MonsterMovement>(out var mm))
            mm.speed = Random.Range(monsterSpeedRange.x, monsterSpeedRange.y);

        NetworkServer.Spawn(go);
        currentMonsters.Add(go);
    }
}
