using Mirror;
using Mirror.Discovery;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject lobbyPanel;

    [Header("Network")]
    [SerializeField] private TMP_InputField addressInput;
    private NetworkRoomManager roomManager;

    [Header("Player UI References")]
    public TMP_Text playerOneName;
    public TMP_Text playerTwoName;
    public Image playerOneStatus;
    public Image playerTwoStatus;

    [Header("Lobby Buttons")]
    public Button readyButton;

    [Header("LAN Discovery")]
    public LANDiscovery discovery;
    public GameObject serverListPanel;
    public Transform serverListParent; // A vertical layout group
    public GameObject serverButtonPrefab;
    private void Awake()
    {
        roomManager = FindAnyObjectByType<NetworkRoomManager>();
        //if (roomManager == null)
        //{
        //    Debug.Log("Cannot find Room Manager");
        //}
        //else
        //{
        //    Debug.Log("Found!");
        //}
    }
    void Start()
    {
        discovery.OnServerFoundEvent += OnServerFound;
    }

    private void Update()
    {
        UpdatePlayerListUI();
    }
    public void Host()
    {
        if (roomManager != null)
        {
            roomManager.StartHost();
            discovery.AdvertiseServer();
            SetLobbyActive();
        }
    }

    public void FindLANGames()
    {
        // clear old list
        foreach (Transform child in serverListParent) Destroy(child.gameObject);

        Debug.Log("Searching for games...");
        menuPanel.SetActive(false);
        serverListPanel.SetActive(true);
        discovery.StartDiscovery();
    }

    void OnServerFound(ServerResponse info)
    {
        Debug.Log("Found server: " + info.uri);

        var btnObj = Instantiate(serverButtonPrefab, serverListParent);

        // Use a safe address string
        string addr = info.EndPoint != null ? info.EndPoint.Address.ToString() : info.uri.ToString();

        // Get TMP_Text safely (or use legacy Text if that's what your prefab has)
        var label = btnObj.GetComponentInChildren<TMPro.TMP_Text>();
        if (label != null)
            label.text = addr;

        var button = btnObj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                discovery.StopDiscovery();
                roomManager.StartClient(info.uri); // use the right singleton
                SetLobbyActive();
            });
        }
    }

    public void Join()
    {
        if (roomManager != null && !string.IsNullOrEmpty(addressInput.text))
        {
            roomManager.networkAddress = addressInput.text;
            roomManager.StartClient();
            SetLobbyActive();
        }
    }

    private void SetLobbyActive()
    {
        menuPanel.SetActive(false);
        serverListPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        playerOneName.text = "Waiting for Player...";
        playerTwoName.text = "Waiting for Player...";
        UpdatePlayerListUI();
    }

    public void Quit()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    public void UpdatePlayerListUI()
    {
        var players = roomManager.roomSlots;
        var playerList = new List<NetworkRoomPlayer>(players);
        playerOneStatus.color = Color.gray;
        playerTwoStatus.color = Color.gray;

        if (players.Count > 0 && playerList[0] != null)
        {
            var player1 = playerList[0];
            playerOneName.text = player1.isLocalPlayer ? "You" : "Player 1";
            playerOneStatus.color = player1.readyToBegin ? Color.green : Color.gray;
        }

        if (players.Count > 1 && playerList[1] != null)
        {
            var player2 = playerList[1];
            playerTwoName.text = player2.isLocalPlayer ? "You" : "Player 2";
            playerTwoStatus.color = player2.readyToBegin ? Color.green : Color.gray;
        }
        else
        {
            playerTwoStatus.color = Color.yellow;
        }

        if (readyButton != null)
        {
            readyButton.interactable = true;
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
                roomPlayer.CmdChangeReadyState(!roomPlayer.readyToBegin);
            }
        }
    }
}