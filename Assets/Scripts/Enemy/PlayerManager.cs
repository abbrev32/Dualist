using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    public static List<PlayerManager> allPlayers = new List<PlayerManager>();

    public override void OnStartServer()
    {
        allPlayers.Add(this);
    }

    public override void OnStopServer()
    {
        allPlayers.Remove(this);
    }
}