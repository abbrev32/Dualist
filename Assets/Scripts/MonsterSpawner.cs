using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : NetworkBehaviour
{
    public GameObject monsterPrefab;
    public int monstersPerWave = 5;
    public float spawnDelay = 0.5f;
    public Vector2 speedRange = new Vector2(2f, 5f);
    public float positionOffset = 1.5f;
    public int numWave = 0; // Number of waves
    public beACollectable collectable;

    private List<GameObject> currentMonsters = new List<GameObject>();

    public override void OnStartServer()
    {
        SpawnWave();
    }

    void Update()
    {
        if (!isServer) return;

        // Clean up destroyed monsters
        currentMonsters.RemoveAll(monster => monster == null);

        // If all monsters are dead, wave is complete
        if (currentMonsters.Count == 0 && numWave !=4)
        {
            numWave++;       // increment wave count
            CheckWave();     // spawn collectible if needed
            if (numWave < 3) // spawn next wave only if not reached max
            {
                SpawnWave();
            }
        }
    }

    [Server]
    public void CheckWave()
    {
        if (numWave == 3 && collectable != null)
        {
            collectable.TrySpawnCollectable();
        }
    }

    void SpawnWave()
    {
        StartCoroutine(SpawnMonsters());
    }

    System.Collections.IEnumerator SpawnMonsters()
    {
        for (int i = 0; i < monstersPerWave; i++)
        {
            Vector3 spawnPos = transform.position;
            spawnPos.y += Random.Range(-positionOffset, positionOffset);

            GameObject newMonster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);

            MonsterMovement moveScript = newMonster.GetComponent<MonsterMovement>();
            if (moveScript != null)
            {
                moveScript.speed = Random.Range(speedRange.x, speedRange.y);
            }

            NetworkServer.Spawn(newMonster);
            currentMonsters.Add(newMonster);

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
