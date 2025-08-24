using UnityEngine;
using Mirror;

public class TurretHealth : NetworkBehaviour
{
    public Sprite fullHealthSprite; // make sure this exists
    public Sprite halfHealthSprite;
    public Sprite lowHealthSprite;

    [SyncVar]
    public float maxHealth = 3;

    [SyncVar(hook = nameof(OnCurrentHealthChanged))]
    public float currentHealth = 3;

    public AudioSource audioSource;
    public AudioClip destroySound;

    // Cooldown
    private float lastDamageTime = -Mathf.Infinity;
    public float damageCooldown = 1.5f;

    // Called on ALL clients whenever currentHealth changes
    void OnCurrentHealthChanged(float oldHealth, float newHealth)
    {
        UpdateSprite(newHealth);

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
        if (Time.time - lastDamageTime < damageCooldown)
            return; // still in cooldown

        lastDamageTime = Time.time;

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
