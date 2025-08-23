using Mirror;
using UnityEngine;

[RequireComponent(typeof(PlayerFaction))]
public class PlayerColor : NetworkBehaviour
{
    private PlayerFaction factionComponent;
    private SpriteRenderer[] allSprites;

    // Gray colors for self
    private static readonly Color DarkSelf = new Color(0.1f, 0.1f, 0.1f, 1f);
    private static readonly Color LightSelf = new Color(0.9f, 0.9f, 0.9f, 1f);
    // Black/white for others
    private static readonly Color DarkOther = Color.black;
    private static readonly Color LightOther = Color.white;

    void Awake()
    {
        factionComponent = GetComponent<PlayerFaction>();
        allSprites = GetComponentsInChildren<SpriteRenderer>(true);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        // Ensure color applies when the faction SyncVar arrives/changes
        ApplyColors();
    }

    // You can also hook into SyncVar if you modify PlayerFaction:
    // [SyncVar(hook = nameof(OnFactionChanged))]
    // public Faction faction;
    // void OnFactionChanged(Faction oldF, Faction newF) { ApplyColors(); }

    public void ApplyColors()
    {
        // If faction hasn't been assigned yet, bail out
        var faction = factionComponent.faction;
        Color targetColor;

        bool isSelf = isLocalPlayer;

        if (isSelf)
        {
            // Self gray colors depending on faction
            targetColor = (faction == PlayerFaction.Faction.Dark) ? DarkSelf : LightSelf;
        }
        else
        {
            // Others are pure black or white depending on THEIR faction
            targetColor = (faction == PlayerFaction.Faction.Dark) ? DarkOther : LightOther;
        }

        foreach (var sr in allSprites)
        {
            sr.color = targetColor;
        }
    }
}
