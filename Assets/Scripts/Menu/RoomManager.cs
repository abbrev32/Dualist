using UnityEngine;
using Mirror;

public class RoomManager : NetworkRoomManager
{
    public Transform spawnPoint;

    public override void OnRoomServerPlayersReady()
    {
        if (allPlayersReady)
        {
            Debug.Log("All players ready. Starting Level1...");
            ServerChangeScene("SampleScene"); // loads Level1
        }
    }

}