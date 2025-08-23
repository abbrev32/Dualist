using Mirror;
using UnityEngine;

[RequireComponent(typeof(PlayerFaction))]
public class PlayerColor : NetworkBehaviour
{
    private PlayerFaction faction;
    private SpriteRenderer[] allSprites;

    // You can tweak these colors if you want slightly different shades.
    private Color darkSelf = new Color(0.3f, 0.3f, 0.3f, 1f);   // Dark Gray
    private Color lightSelf = new Color(0.8f, 0.8f, 0.8f, 1f);  // Light Gray
    private Color darkOther = Color.black;
    private Color lightOther = Color.white;

    void Start()
    {
        faction = GetComponent<PlayerFaction>();
        allSprites = GetComponentsInChildren<SpriteRenderer>(true);

        ApplyColors();
    }

    void ApplyColors()
    {
        bool isSelf = isLocalPlayer; // or hasAuthority depending on your net setup
        Color targetColor;

        if (isSelf)
        {
            // Local player sees themselves as gray (depending on faction)
            if (faction.faction == PlayerFaction.Faction.Dark)
                targetColor = darkSelf;
            else
                targetColor = lightSelf;
        }
        else
        {
            // Local player sees others as pure black/white depending on their faction
            if (faction.faction == PlayerFaction.Faction.Dark)
                targetColor = darkOther;
            else
                targetColor = lightOther;
        }

        foreach (var sr in allSprites)
        {
            sr.color = targetColor;
        }
    }
}
