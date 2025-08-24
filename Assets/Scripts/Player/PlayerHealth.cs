using UnityEngine;
using Mirror;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

public class PlayerHealth : NetworkBehaviour
{
    public float maxHealth = 10;
    

    [SyncVar(hook = nameof(OnHealthChange))]
    public float currentHealth = 1;
    public static Slider healthBarSelf;
    public static Slider healthBarOther;


    [SyncVar]
    public bool isDead = false;

    public Transform spawnPoint;

    // Audio Components
    public AudioClip deathSound;
    private AudioSource audioSource;

    private void Awake()
    {
        healthBarSelf = GameObject.Find("HealthBarSelf").GetComponent<Slider>();
        healthBarOther = GameObject.Find("HealthBarOther").GetComponent<Slider>();

        if (healthBarSelf != null) healthBarSelf.maxValue = maxHealth;
        if (healthBarOther != null) healthBarOther.maxValue = maxHealth;

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
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
        if (isDead) return;
        // Apply damage first
        currentHealth -= damage;


        if (currentHealth <= 0)
        {
          

            if (!EntityChecker.nextLevel)
            {
                RpcOnPlayerDeath();
            }
            else
            {
                RpcTheEnd();
            }
            //StartCoroutine(ServerDestroyAfterDelay());
        }
    }
    [ClientRpc]
    private void RpcTheEnd()
    {
        StartCoroutine(HandleDeathSequence2());
    }

    [Server]
    public void ServerRespawn()
    {
        currentHealth = maxHealth;
        isDead = false;

        //Transform startPos = NetworkManager.singleton.GetStartPosition();
        Vector3 spawnPosition = spawnPoint.position;

        RpcRespawn(spawnPosition);
    }

    [ClientRpc]
    public void RpcRespawn(Vector3 position)
    {
        transform.position = position;
        // GetComponent<PlayerMovement>().enabled = true;
    }

    public void HealthReset()
    {
        currentHealth = maxHealth;
    }

    [ClientRpc]
    void RpcOnPlayerDeath()
    {
        Debug.Log("Player Died!");
        // Start the new coroutine to handle the death sequence
        StartCoroutine(HandleDeathSequence());
    }

    private IEnumerator HandleDeathSequence2()
    {
        isDead = true;
        // Play the death sound
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

      

        // Wait for a few seconds before showing the game over screen
        yield return new WaitForSeconds(1f);

        Time.timeScale = 0;
        // Show the game over screen
        //todo
        FindAnyObjectByType<GameManager>().ShowTheEnd();
    }    
    private IEnumerator HandleDeathSequence()
    {   
        isDead = true;
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

       

        // Wait for a few seconds before showing the game over screen
        yield return new WaitForSeconds(1f);

        Time.timeScale = 0;
        // Show the game over screen
        FindAnyObjectByType<GameManager>().ShowGameOverScreen();
      // Play the death sound
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
    internal bool pvp;

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