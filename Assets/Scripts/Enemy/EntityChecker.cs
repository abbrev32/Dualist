using Mirror;
using UnityEngine;

public class EntityChecker : NetworkBehaviour
{
    public static bool nextLevel = false;
    public TurretSpawner turretSpawner;   // Assign in Inspector
    public MonsterSpawner monsterSpawner; // Assign in Inspector
    public GameManager gameManager;       // Assign in Inspector

    void Update()
    {
        if (!isServer) return; // only server controls the logic

        // Manual override with O key (for testing)
        if (!nextLevel)
        {
            // Auto check: all turrets destroyed + portal dead
            if (turretSpawner != null && monsterSpawner != null)
            {
                if (turretSpawner.RemainingCount == 0 && monsterSpawner.portaldeath)
                {
                    RpcLevelClear();
                    nextLevel = true;
                }
            }
        }
    }

    [ClientRpc]
    public void RpcLevelClear()
    {
        if (gameManager != null)
        {
            gameManager.LevelClear();
        }
    }
}

