using UnityEngine;
using Mirror;
using UnityEngine.UI;
public class CutSceneManager : NetworkBehaviour
{

    public GameObject cutscenePanel;
    public GameObject cutscenePanelFirst;
    public Button nextButton;
    void Start()
    {
        Time.timeScale = 0f;
        cutscenePanelFirst.SetActive(true);
    }

    void Update()
    {
        if (cutscenePanel.activeInHierarchy || cutscenePanelFirst.activeInHierarchy)
        {
            Time.timeScale = 0f;
        }
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if (!isLocalPlayer) return;
    //     if (isServer)
    //     {
    //         nextButton.gameObject.SetActive(true);
    //     }
    // }
    public void OnFirstNext()
    {
        RpcOnFirstNext();
    }
    [ClientRpc]
    private void RpcOnFirstNext()
    {
        cutscenePanelFirst.SetActive(false);
        cutscenePanel.SetActive(true);
    }
    public void OnNext()
    {
        RpcOnNext();
    }
    [ClientRpc]
    private void RpcOnNext()
    {
        Time.timeScale = 1f;
        cutscenePanel.SetActive(false);
    }
}
