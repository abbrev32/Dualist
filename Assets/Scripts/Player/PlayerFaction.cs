using Mirror;
using UnityEngine;

public class PlayerFaction : NetworkBehaviour
{
    public enum Faction { Dark, Light }

    [SyncVar]
    public Faction faction;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdAssignFaction(isServer ? Faction.Dark : Faction.Light);
        Debug.Log("My Faction: " +  faction);
    }

    [Command]
    void CmdAssignFaction(Faction f)
    {
        faction = f;
    }
}
