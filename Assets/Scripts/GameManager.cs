using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using System.Collections;

[RequireComponent(typeof(PlayerHealth))]
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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RpcOnPause();
        }
    }
    [ClientRpc]
    public void RpcOnPause()
    {
        Time.timeScale = 0f;
    }
    //TODO HERE
    public void OnNextLevel()
    {
        Time.timeScale = 1.0f;
        RpcHideOverlay(levelClearUI);
        GameObject player = GameObject.Find("RealPlayer(Clone)");
        player.GetComponent<PlayerHealth>().ServerRespawn();
        //GetComponent<PlayerHealth>().ServerRespawn();
        SwordScript.pvp = true;
    }
    [ClientRpc]
    public void RpcHideOverlay(GameObject panelUI)
    {
        panelUI.SetActive(false);
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
        // Instead of running the logic directly, start the shutdown coroutine.
        StartCoroutine(ShutdownAndReturnToMenu());
    }

    private IEnumerator ShutdownAndReturnToMenu()
    {
        Time.timeScale = 1f;

        if (NetworkManager.singleton != null)
        {
            // First, stop the host/client/server.
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

            // Wait a very short moment to allow network processes to stop.
            yield return new WaitForSeconds(0.1f);

            // Now, destroy the manager's GameObject.
            Destroy(NetworkManager.singleton.gameObject);

            // **This is the crucial step:** Wait until the end of the frame.
            // This ensures the Destroy() command has been fully executed
            // before we try to load the next scene.
            yield return new WaitForEndOfFrame();
        }

        // Finally, load the main menu scene.
        SceneManager.LoadScene("MainMenu");
    }


}