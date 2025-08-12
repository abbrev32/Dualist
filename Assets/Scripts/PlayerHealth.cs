using UnityEngine;
using Mirror;
using TMPro;
using System;
public class PlayerHealth : NetworkBehaviour
{
    public float maxHealth = 10;

    [SyncVar(hook=nameof(OnHealthChange))]
    public float currentHealth = 1;
    public static TextMeshProUGUI healthText;
    public static TextMeshProUGUI healthTextOther;


    private void Awake()
    {
        healthText = GameObject.Find("HealthText").GetComponent<TextMeshProUGUI>();
        healthTextOther = GameObject.Find("HealthTextOther").GetComponent<TextMeshProUGUI>();
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
            Console.WriteLine("Damage Taken \n");
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
        if(isLocalPlayer && currentHealth > 0)
            currentHealth -= damage;
    }
    public void OnHealthChange(float oldHealth, float newHealth)
    {
        if (isLocalPlayer && healthText != null)
        {
            healthText.text = newHealth.ToString();
        }
        if (!isLocalPlayer && healthTextOther != null)
        {
            healthTextOther.text = newHealth.ToString();
        }
    }
}
