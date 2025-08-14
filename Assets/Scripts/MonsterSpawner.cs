using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : NetworkBehaviour
{
    public GameObject monsterPrefab;   // Your monster prefab
    public int monstersPerWave = 5;
    public float spawnDelay = 0.5f;     // Time between monster spawns
    public Vector2 speedRange = new Vector2(2f, 5f); // Random speed range
    public float positionOffset = 1.5f; // How far apart they spawn

    private List<GameObject> currentMonsters = new List<GameObject>();

    public override void OnStartServer()
    {
        SpawnWave();
    }

    void Update()
    {
        if(!isServer) return;

        // Remove destroyed monsters from the list
        currentMonsters.RemoveAll(monster => monster == null);

        // If all monsters are dead, spawn again
        if (currentMonsters.Count == 0)
        {
            SpawnWave();
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
            // Spawn position with a random offset (so they don’t overlap)
            Vector3 spawnPos = transform.position;
            spawnPos.y += Random.Range(-positionOffset, positionOffset); // Slight vertical difference

            GameObject newMonster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);

            // Assign random speed
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
