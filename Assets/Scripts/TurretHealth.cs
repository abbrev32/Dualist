using UnityEngine;
using Mirror;
public class TurretHealth : NetworkBehaviour
{
    [SyncVar]
    public float maxHealth = 3;
    public float currentHealth = 3;

    public void CmdTakeDamage(float damage) //testing functionality
    {
        TakeDamage(damage);
    }
    public void TakeDamage(float damage)
    {
        if (currentHealth > 1)
            currentHealth -= damage;
        else
        {
            currentHealth -= damage;
            Kill();
        }
    }
    [Server]
    public void Kill()
    {
        NetworkServer.Destroy(gameObject);
    }
}