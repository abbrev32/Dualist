using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnRetryVoteChanged))] private bool player1RetryVote;
    [SyncVar(hook = nameof(OnRetryVoteChanged))] private bool player2RetryVote;

    [SyncVar(hook = nameof(OnQuitVoteChanged))] private bool player1QuitVote;
    [SyncVar(hook = nameof(OnQuitVoteChanged))] private bool player2QuitVote;

    // Assign these from inspector or find them dynamically on clients
    public GameOverUI ui;

    public GameObject gameOverPanel;

    public void ShowGameOverScreen()
    {
        gameOverPanel.SetActive(true);
    }
    [Command(requiresAuthority = false)]
    public void CmdVoteRetry(NetworkConnectionToClient sender = null)
    {
        if (sender.identity == null) return;
        if (sender.identity.netId == NetworkServer.connections[0].identity.netId)
            player1RetryVote = !player1RetryVote;
        else
            player2RetryVote = !player2RetryVote;

        CheckForFullVote();
    }

    [Command(requiresAuthority = false)]
    public void CmdVoteQuit(NetworkConnectionToClient sender = null)
    {
        if (sender.identity == null) return;
        if (sender.identity.netId == NetworkServer.connections[0].identity.netId)
            player1QuitVote = !player1QuitVote;
        else
            player2QuitVote = !player2QuitVote;

        CheckForFullVote();
    }

    void CheckForFullVote()
    {
        // if both voted Retry
        if (player1RetryVote && player2RetryVote)
        {
            RpcRestartMatch();
        }
        // if both voted Quit
        if (player1QuitVote && player2QuitVote)
        {
            RpcQuitMatch();
        }
    }

    [ClientRpc]
    void RpcRestartMatch()
    {
        Debug.Log("Reastart");
        // logic to restart
    }

    [ClientRpc]
    void RpcQuitMatch()
    {
        Debug.Log("Quit");
        // logic to quit
    }

    void OnRetryVoteChanged(bool oldValue, bool newValue)
    {
        ui.UpdateRetryUI(player1RetryVote, player2RetryVote);
    }

    void OnQuitVoteChanged(bool oldValue, bool newValue)
    {
        ui.UpdateQuitUI(player1QuitVote, player2QuitVote);
    }
}
