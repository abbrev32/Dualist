using UnityEngine;
using Mirror;
using UnityEngine.UI;
public class CutSceneManager : NetworkBehaviour
{

    public GameObject cutscenePanel;
    public Button nextButton;
    void Start()
    {
        Time.timeScale = 0f;
        cutscenePanel.SetActive(true);
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
