using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviour
{
    [Header("Player UI References")]
    public TMP_Text playerOneName;
    public TMP_Text playerTwoName;
    public Image playerOneStatus;
    public Image playerTwoStatus;

    [Header("Lobby Buttons")]
    public Button readyButton;
    public Button startButton;

    private NetworkRoomManager roomManager;

    void Start()
    {
        roomManager = FindAnyObjectByType<NetworkRoomManager>();

        if (readyButton == null)
        {
            readyButton = GameObject.Find("Ready").GetComponent<Button>();
        }

        readyButton.interactable = true;
        readyButton.onClick.AddListener(OnReady);

        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStart);
        }
        UpdatePlayerListUI();
    }

    private void Update()
    {
        UpdatePlayerListUI();
    }

    public void UpdatePlayerListUI()
    {
        playerOneName.text = "Waiting for Player...";
        playerTwoName.text = "Waiting for Player...";
        playerOneStatus.color = Color.gray;
        playerTwoStatus.color = Color.gray;

        var players = roomManager.roomSlots;
        var playerList = new List<NetworkRoomPlayer>(players);

        // Find local player and server player
        NetworkRoomPlayer localPlayer = null;
        NetworkRoomPlayer serverPlayer = null;
        foreach (var p in playerList)
        {
            if (p != null)
            {
                if (p.isLocalPlayer)
                    localPlayer = p;
                if (p.isServer)
                    serverPlayer = p;
            }
        }

        // Assign names and status
        if (playerList.Count > 0 && playerList[0] != null)
        {
            var player1 = playerList[0];
            playerOneName.text = player1.isLocalPlayer ? "You" : "Player 1";
            playerOneStatus.color = player1.readyToBegin ? Color.green : Color.gray;
        }
        if (playerList.Count > 1 && playerList[1] != null)
        {
            var player2 = playerList[1];
            playerTwoName.text = player2.isLocalPlayer ? "You" : "Player 2";
            playerTwoStatus.color = player2.readyToBegin ? Color.green : Color.gray;
        }
        else
        {
            playerTwoName.text = "Waiting for Player...";
            playerTwoStatus.color = Color.yellow;
        }

        // Enable start button only for host (server) and when all players are ready
        if (startButton != null)
        {
            bool showStart = false;
            if (localPlayer != null && localPlayer.isServer)
            {
                showStart = roomManager.allPlayersReady;
            }
            startButton.interactable = showStart;
        }
    }

    public void OnReady()
    {
        Debug.Log("Ready Pressed!");
        if (NetworkClient.localPlayer != null)
        {
            if (NetworkClient.localPlayer.TryGetComponent<NetworkRoomPlayer>(out var roomPlayer))
            {
                Debug.Log($"IsLocalPlayer: {NetworkClient.localPlayer.isLocalPlayer}");
                Debug.Log($"Has NetworkRoomPlayer: {NetworkClient.localPlayer.TryGetComponent<NetworkRoomPlayer>(out var _)}");

                roomPlayer.CmdChangeReadyState(!roomPlayer.readyToBegin);
            }
        }
    }

    public void OnStart()
    {
        if (roomManager.allPlayersReady && roomManager.isNetworkActive)
        {
            roomManager.ServerChangeScene(roomManager.GameplayScene);
        }
    }
}
