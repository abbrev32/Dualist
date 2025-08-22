using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public Image retryVoteP1;
    public Image retryVoteP2;
    public Image quitVoteP1;
    public Image quitVoteP2;

    void Start()
    {
        retryVoteP1.color = Color.green;
        retryVoteP2.color = Color.green;
        quitVoteP1.color = Color.green;
        quitVoteP2.color = Color.green;
        retryVoteP1.enabled = false;
        retryVoteP2.enabled = false;
        quitVoteP1.enabled = false;
        quitVoteP2.enabled = false;
    }

    public void UpdateRetryUI(bool p1, bool p2)
    {
        retryVoteP1.enabled = p1;
        retryVoteP2.enabled = p2;
    }

    public void UpdateQuitUI(bool p1, bool p2)
    {
        quitVoteP1.enabled = p1;
        quitVoteP2.enabled = p2;
    }
}
