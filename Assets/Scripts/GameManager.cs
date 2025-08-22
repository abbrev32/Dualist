using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public GameObject gameOverUI; // assign in inspector

    public void ShowGameOverScreen()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f; // optional: pause the game
    }

    // Host calls these buttons
    public void OnRetryButton()
    {
        if (!isServer) return;

        Time.timeScale = 1f;
        gameOverUI.SetActive(false);

        // Find every active player in the scene on the server
        PlayerHealth[] players = FindObjectsOfType<PlayerHealth>();
        foreach (PlayerHealth player in players)
        {
            // Call a new server-authoritative respawn function
            player.ServerRespawn();
        }
    }

    public void OnQuitButton()
    {
        if (!isServer) return;
        Time.timeScale = 1f;
        // return to lobby/main menu
        NetworkManager.singleton.ServerChangeScene("MainMenu");
    }
}
