using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

public class GameManager : NetworkBehaviour
{
    public GameObject levelClearUI;
    public TMP_Text waitText;
    public Button nextButton;
    public GameObject gameOverUI; // assign in inspector
    public Button retryButton;
    public Button quitButton;

    public NetworkRoomManager roomManager;

    public void LevelClear()
    {
        levelClearUI.SetActive(true);
        Time.timeScale = 0;
        if (isServer)
        {
            nextButton.gameObject.SetActive(true);
        }
        else
        {
            waitText.gameObject.SetActive(true);
        }
    }
    //TODO HERE
    public void OnNextLevel()
    {
        Time.timeScale = 1.0f;
        GameObject player = GameObject.Find("RealPlayer");
        player.GetComponent<PlayerHealth>().ServerRespawn();
        SwordScript.pvp = true;
        levelClearUI.SetActive(false);
    }
    public void ShowGameOverScreen()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f; // optional: pause the game
    }

    public override void OnStartServer()
    {
        // On the server, ensure the button is visible.
        retryButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
    }
    public override void OnStartClient()
    {
        // On clients that are not the server, hide the retry button.
        if (!isServer)
        {
            retryButton.gameObject.SetActive(false);
        }
        quitButton.gameObject.SetActive(true);
    }


    // This function is only called by the Host's button.
    public void OnRetryButton()
    {
        // 1. Make sure only the server can run this.
        if (!isServer) return;

        // 2. Respawn all players (this logic is server-authoritative and correct).
        PlayerHealth[] players = FindObjectsOfType<PlayerHealth>();
        foreach (PlayerHealth player in players)
        {
            player.ServerRespawn();
        }

        // 3. Instead of hiding the UI here, call the ClientRpc for all clients.
        RpcHideGameOverScreen();
    }

    [ClientRpc]
    private void RpcHideGameOverScreen()
    {
        // 4. This code now runs on the Host AND all Clients.
        Debug.Log("Hiding Game Over screen for all clients.");
        gameOverUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnQuitButton()
    {
        Time.timeScale = 1f;

        // Properly shut down networking first
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
        else if (NetworkServer.active)
        {
            NetworkManager.singleton.StopServer();
        }

        // Explicitly destroy the room manager singleton if you want a full reset
        if (NetworkManager.singleton != null)
        {
            Destroy(NetworkManager.singleton.gameObject);
        }

        // Now load the main menu
        SceneManager.LoadScene("MainMenu");
    }

}