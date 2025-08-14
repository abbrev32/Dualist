using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab; // Assign your Monster prefab in Inspector
    public float spawnInterval = 3f; // Time between spawns

    private void Start()
    {
        SpawnMonster();
    }

    public void SpawnMonster()
    {
        Instantiate(monsterPrefab, transform.position, Quaternion.identity);
        Invoke(nameof(SpawnMonster), spawnInterval);
    }
}