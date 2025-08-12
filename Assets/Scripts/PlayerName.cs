using Mirror;
using TMPro;
using UnityEngine;

public class PlayerName : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;
    private TextMeshPro nameText;

    private void Awake()
    {
        nameText = GetComponentInChildren<TextMeshPro>();
    }
    public override void OnStartLocalPlayer()
    {
        string hostName = "Player_" + Random.Range(1, 100);
        CmdSetName(hostName);
    }

    public override void OnStartClient()
    {
        OnNameChanged("", playerName);
    }
    [Command]
    public void CmdSetName(string name)
    {
        playerName = name;
    }
    public void OnNameChanged(string oldName, string newName)
    {
        nameText.text = newName;
    }
}
