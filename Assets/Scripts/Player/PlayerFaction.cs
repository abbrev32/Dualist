using Mirror;
using UnityEngine;

[RequireComponent(typeof(PlayerColor))]
public class PlayerFaction : NetworkBehaviour
{
    public enum Faction { Dark, Light }

    [SyncVar(hook = nameof(OnFactionChanged))]
    public Faction faction;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdAssignFaction(isServer ? Faction.Dark : Faction.Light);
        Debug.Log("My Faction: " + faction);
    }

    [Command]
    void CmdAssignFaction(Faction f)
    {
        faction = f;
    }
    void OnFactionChanged(Faction oldFaction, Faction newFaction)
    {
        // Inform PlayerColor that faction changed
        GetComponent<PlayerColor>()?.ApplyColors();
    }
}
