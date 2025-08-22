using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public GameObject gameOverUI; // assign in inspector
    public Button retryButton;
    public Button quitButton;
    public void ShowGameOverScreen()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f; // optional: pause the game
    }

    private void Awake()
    {
        if (!isServer)
        {
            retryButton.gameObject.SetActive(false);
        }
        else
        {
            retryButton.gameObject.SetActive(true);
        }
        quitButton.gameObject.SetActive(true);
    }

    // Host calls these buttons
    public void OnRetryButton()
    {
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
        Time.timeScale = 1f;
        // return to lobby/main menu
        NetworkManager.singleton.ServerChangeScene("MainMenu");
    }
}
