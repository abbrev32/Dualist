using UnityEngine;
using Mirror;

public class TurretHealth : NetworkBehaviour
{
    public Sprite fullHealthSprite;
    public Sprite halfHealthSprite;
    public Sprite lowHealthSprite;

    [SyncVar]
    public float maxHealth = 3;

    [SyncVar(hook = nameof(OnCurrentHealthChanged))]
    public float currentHealth = 3;

    public AudioSource audioSource;
    public AudioClip destroySound;

    // Called on ALL clients whenever currentHealth changes
    void OnCurrentHealthChanged(float oldHealth, float newHealth)
    {
        // Update sprite on all clients
        UpdateSprite(newHealth);

        // Check if destroyed
        if (newHealth <= 0 && oldHealth > 0)
        {
            if (audioSource != null && destroySound != null)
                audioSource.PlayOneShot(destroySound);
        }
    }

    [Command]
    public void CmdTakeDamage(float damage)
    {
        TakeDamage(damage);
    }

    [Server]
    public void TakeDamage(float damage)
    {
        if (currentHealth > 0)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                RpcPlayDestroySound();
                Kill();
            }
        }
    }

    void UpdateSprite(float health)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        if (health >= 3)
            spriteRenderer.sprite = fullHealthSprite;
        else if (health == 2)
            spriteRenderer.sprite = halfHealthSprite;
        else if (health == 1)
            spriteRenderer.sprite = lowHealthSprite;
    }

    [ClientRpc]
    public void RpcPlayDestroySound()
    {
        if (audioSource != null && destroySound != null)
        {
            audioSource.PlayOneShot(destroySound);
        }
    }

    [Server]
    public void Kill()
    {
        NetworkServer.Destroy(gameObject);
    }
}
