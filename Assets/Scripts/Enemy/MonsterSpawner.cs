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
    public int numWave = 0;
    public beACollectable collectable;

    // 👇 New variables for audio
    public AudioSource audioSource;
    public AudioClip destroySoundClip;

    private List<GameObject> currentMonsters = new List<GameObject>();

    [SyncVar]
    public int health = 10; // spawner health

    public override void OnStartServer()
    {
        SpawnWave();
    }

    void Update()
    {
        if (!isServer) return;

        currentMonsters.RemoveAll(monster => monster == null);

        if (currentMonsters.Count == 0 && numWave != 4)
        {
            numWave++;
            CheckWave();
            if (numWave < 3)
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

    // 👇 Updated Damage System with sound
    [Server]
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            // Call the RPC to play the sound on all clients
            RpcPlayDestroySound();

            // Wait a moment for the sound to play before destroying the object
            // You can adjust the delay based on the length of your sound clip
            Invoke(nameof(DelayedDestroy), 1.0f);
        }
    }

    [ClientRpc]
    void RpcPlayDestroySound()
    {
        // Play the sound if the audio source and clip are set
        if (audioSource != null && destroySoundClip != null)
        {
            audioSource.PlayOneShot(destroySoundClip);
        }
    }

    [Server]
    void DelayedDestroy()
    {
        NetworkServer.Destroy(gameObject); // destroy spawner across network
    }
}