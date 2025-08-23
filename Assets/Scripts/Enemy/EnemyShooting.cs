using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bullet;
    public Transform bulletPos;
    public float shootingInterval = 2f;

    [Header("Player Targeting")]
    public int targetPlayerNumber = 1; // Set this to 1 or 2 in the Inspector

    private float timer;
    private Transform target;

    void Update()
    {
        FindAssignedPlayer();

        timer += Time.deltaTime;
        if (timer >= shootingInterval && target != null)
        {
            timer = 0;
            Shoot();
        }
    }

    void FindAssignedPlayer()
    {
        // Find all players
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            target = null;
            return;
        }

        // Convert player number to index (Player 1 = index 0, Player 2 = index 1)
        int targetIndex = targetPlayerNumber - 1;

        // Make sure the index is valid
        if (targetIndex >= 0 && targetIndex < players.Length)
        {
            target = players[targetIndex].transform;
        }
        else
        {
            // Fallback: target the first player if the assigned number is invalid
            target = players[0].transform;
            Debug.LogWarning("Invalid player number assigned. Targeting Player 1 instead.");
        }
    }

    void Shoot()
    {
        if (bullet != null && bulletPos != null && target != null)
        {
            GameObject newBullet = Instantiate(bullet, bulletPos.position, Quaternion.identity);
            EnemyBullet bulletScript = newBullet.GetComponent<EnemyBullet>();

            if (bulletScript != null)
            {
                bulletScript.SetTarget(target);
            }
        }
    }
}