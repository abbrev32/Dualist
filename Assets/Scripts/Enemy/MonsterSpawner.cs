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
    public float ghostSpawnDelay = 3f;   // between ghosts

    [Header("Spawn variation")]
    public Vector2 monsterSpeedRange = new Vector2(2f, 5f);
    public float positionOffsetY = 1.5f;

    [Header("Spawner Health")]
    [SyncVar] public int maxHealth = 3;
    [SyncVar] private int currentHealth;

    private Animator anim;
    private NetworkAnimator netAnimator;

    private readonly List<GameObject> currentMonsters = new List<GameObject>();

    // Damage cooldown
    private float lastDamageTime = -Mathf.Infinity;
    public float damageCooldown = 1.5f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        netAnimator = GetComponent<NetworkAnimator>();
    }

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
        SpawnWave();
    }

    private void Update()
    {
        if (!isServer) return;

        // cleanup null entries
        currentMonsters.RemoveAll(m => m == null);

        // auto next wave when all dead
        if (currentMonsters.Count == 0)
            SpawnWave();
    }

    // --- Damage ---
    [Server]
    public void TakeDamage(int amount)
    {
        // cooldown check
        if (Time.time - lastDamageTime < damageCooldown) return;
        lastDamageTime = Time.time;

        if (currentHealth <= 0) return;

        netAnimator?.SetTrigger("Damage");
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            KillSpawner();
        }
    }

    [Server]
    private void KillSpawner()
    {
        // despawn all monsters from this portal
        foreach (var m in currentMonsters)
            if (m != null) NetworkServer.Destroy(m);

        currentMonsters.Clear();
        NetworkServer.Destroy(gameObject); // destroy portal itself
    }

    // --- Spawning ---
    [Server]
    private void SpawnWave()
    {
        StartCoroutine(SpawnWaveRoutine());
    }

    [Server]
    private IEnumerator SpawnWaveRoutine()
    {
        // spawn monsters first (short delay)
        for (int i = 0; i < monstersPerWave; i++)
        {
            SpawnOne(monsterPrefab, trySetMonsterSpeed: true);
            yield return new WaitForSeconds(monsterSpawnDelay);
        }

        // then ghosts (3s delay each)
        for (int i = 0; i < ghostsPerWave; i++)
        {
            SpawnOne(ghostPrefab, trySetMonsterSpeed: false); // ghosts handle their own movement
            yield return new WaitForSeconds(ghostSpawnDelay);
        }
    }

    [Server]
    private void SpawnOne(GameObject prefab, bool trySetMonsterSpeed)
    {
        Vector3 pos = transform.position;
        pos.y += Random.Range(-positionOffsetY, positionOffsetY);

        GameObject go = Instantiate(prefab, pos, Quaternion.identity);

        // only set speed on normal monsters
        if (trySetMonsterSpeed && go.TryGetComponent<MonsterMovement>(out var mm))
        {
            mm.speed = Random.Range(monsterSpeedRange.x, monsterSpeedRange.y);
        }

        NetworkServer.Spawn(go);
        currentMonsters.Add(go);
    }
}
