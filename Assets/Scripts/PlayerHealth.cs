using UnityEngine;
using Mirror;
using TMPro;
using TMPro.Examples;
using System;
public class PlayerHealth : NetworkBehaviour
{
    public float maxHealth = 10;

    [SyncVar(hook=nameof(OnHealthChange))]
    public float currentHealth = 1;
    [SerializeField]
    private TextMeshProUGUI healthText;

    private void Awake()
    {
        healthText = GameObject.Find("HealthText").GetComponent<TextMeshProUGUI>();
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
    public void CmdTakeDamage(float damage)
    {
        currentHealth -= damage;
    }
    public void OnHealthChange(float oldHealth, float newHealth)
    {
        if (healthText != null)
        {
            healthText.text = newHealth.ToString();
        }
    }
}
