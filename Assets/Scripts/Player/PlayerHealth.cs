using UnityEngine;
using Mirror;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections;
public class PlayerHealth : NetworkBehaviour
{
    public float maxHealth = 10;

    [SyncVar(hook=nameof(OnHealthChange))]
    public float currentHealth = 1;
    public static Slider healthBarSelf;
    public static Slider healthBarOther;
   

    private void Awake()
    {
        healthBarSelf = GameObject.Find("HealthBarSelf").GetComponent<Slider>();
        healthBarOther = GameObject.Find("HealthBarOther").GetComponent<Slider>();

        if(healthBarSelf != null) healthBarSelf.maxValue = maxHealth;
        if(healthBarOther != null) healthBarOther.maxValue = maxHealth;
    }
    public override void OnStartLocalPlayer()
    {
        CmdSetHealth(maxHealth);
    }
    public override void OnStartClient()
    {
        OnHealthChange(0, currentHealth);
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.R))
        {
            CmdTakeDamage(1);
        }
    }

    [Command]
    private void CmdSetHealth(float health)
    {
        currentHealth = health;
    }
    [Command]
    public void CmdTakeDamage(float damage) //testing functionality
    {
        TakeDamage(damage);
    }

    [Server]
    public void TakeDamage(float damage)
    {
        // Apply damage first
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            RpcOnPlayerDeath();
            StartCoroutine(ServerDestroyAfterDelay());
        }
    }
    public void HealthReset()
    {
        currentHealth = maxHealth;
    }

    [ClientRpc]
    void RpcOnPlayerDeath()
    {
        Debug.Log("Player Died!");
        FindAnyObjectByType<GameManager>().ShowGameOverScreen();
    }

    [Server]
    private IEnumerator ServerDestroyAfterDelay()
    {
        // Wait for a fraction of a second to ensure the RPC is sent and processed
        yield return new WaitForSeconds(0.2f);
        NetworkServer.Destroy(gameObject);
    }
    public void OnHealthChange(float oldHealth, float newHealth)
    {
        if (isLocalPlayer && healthBarSelf != null)
        {
            healthBarSelf.value = newHealth;
        }
        if (!isLocalPlayer && healthBarOther != null)
        {
            healthBarOther.value = newHealth;
        }
    }

    //damage taken indicator
    public SpriteRenderer playerSprite; 
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f);
    public float flashDuration = 0.2f; 

    private Color originalColor;

    void Start()
    {
        if (playerSprite == null)
            playerSprite = GetComponent<SpriteRenderer>();

        originalColor = playerSprite.color;
    }

  
    [Client]
    public void TriggerFlash()
    {
      
        
            CmdFlashRed();
        
    }

   
    [Command]
    void CmdFlashRed()
    {
        RpcFlashRed();
    }

   
    [ClientRpc]
    void RpcFlashRed()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRedEffect());
    }

    IEnumerator FlashRedEffect()
    {
        playerSprite.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        playerSprite.color = originalColor;
    }
}
