using UnityEngine;
using Mirror;

public class TurretHealth : NetworkBehaviour
{
    // A [SyncVar] is a networked variable that is synchronized from the server to clients.
    [SyncVar]
    public float maxHealth = 3;

    // A [SyncVar] will automatically update the variable on all clients.
    // However, for immediate action like playing a sound, you often need a hook function.
    [SyncVar(hook = nameof(OnCurrentHealthChanged))]
    public float currentHealth = 3;

    // A reference to the AudioSource component on the same GameObject.
    public AudioSource audioSource;
    // The sound clip to play when the turret is destroyed.
    public AudioClip destroySound;

    // This method is called by the [SyncVar] hook whenever `currentHealth` changes on the server.
    // This allows the change to be processed on all clients.
    void OnCurrentHealthChanged(float oldHealth, float newHealth)
    {
        // Check if the turret has been destroyed.
        if (newHealth <= 0 && oldHealth > 0)
        {
            // Play the destroy sound effect.
            audioSource.PlayOneShot(destroySound);
        }
    }

    // A Command function is sent from the client to the server.
    // The server is the authoritative source for game state changes.
    [Command]
    public void CmdTakeDamage(float damage)
    {
        // Only the server should handle damage calculations.
        TakeDamage(damage);
    }

    // This method handles the damage logic. It is run on the server.
    // You should ensure this method is only called on the server to maintain network authority.
    [Server]
    public void TakeDamage(float damage)
    {
        if (currentHealth > 0)
        {
            // Directly modify the [SyncVar]. Mirror will handle the synchronization.
            currentHealth -= damage;

            // This is a more robust way to handle the "Kill" condition.
            if (currentHealth <= 0)
            {
                // Play the sound on all clients before the object is destroyed.
                // An RPC is a Remote Procedure Call sent from the server to clients.
                RpcPlayDestroySound();

                // Call the Kill method to destroy the object on the server.
                Kill();
            }
        }
    }

    // An RPC (Remote Procedure Call) is sent from the server to all clients.
    [ClientRpc]
    public void RpcPlayDestroySound()
    {
        if (audioSource != null && destroySound != null)
        {
            audioSource.PlayOneShot(destroySound);
        }
    }

    // This method is a [Server] function, meaning it can only be called on the server.
    [Server]
    public void Kill()
    {
        // Destroy the GameObject on the server. Mirror will automatically handle
        // destroying the corresponding object on all clients.
        NetworkServer.Destroy(gameObject);
    }
}