using UnityEngine;
using Mirror;
using TMPro;
using System;
using UnityEngine.UI;
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
    public void TakeDamage(float damage)
    {
        if (currentHealth > 0)
            currentHealth -= damage;
        else
        {
            Destroy();
        }
    }

    [Server]
    public void Destroy()
    {
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
}
