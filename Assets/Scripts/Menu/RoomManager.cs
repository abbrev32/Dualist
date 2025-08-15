using UnityEngine;
using Mirror;

public class RoomManager : NetworkRoomManager
{
    [Header("UI Controller")]
    public MainMenuController menuController;

    public override void OnRoomServerPlayersReady()
    {
        // Only call the UI update function when the game is ready to start
        // This is the correct callback for when all players are ready.
        if (menuController != null)
        {
            menuController.UpdatePlayerListUI();
        }
    }

    public override void OnRoomClientEnter()
    {
        base.OnRoomClientEnter();
        // This is a good place to update the UI when a player first joins
        if (menuController != null)
        {
            menuController.UpdatePlayerListUI();
        }
    }

    public override void OnRoomClientExit()
    {
        base.OnRoomClientExit();
        // This is a good place to update the UI when a player leaves
        if (menuController != null)
        {
            menuController.UpdatePlayerListUI();
        }
    }
}