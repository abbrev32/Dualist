using Mirror;

public class GameOverButtons : NetworkBehaviour
{
    public GameManager manager;

    public void OnRetryButton()
    {
        if (isClient) manager.CmdVoteRetry();
    }

    public void OnQuitButton()
    {
        if (isClient) manager.CmdVoteQuit();
    }
}
