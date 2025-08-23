using Mirror;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class EntityChecker : NetworkBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            RpcLevelClear();
        }
    }
    [ClientRpc]
    public void RpcLevelClear()
    {
        GetComponent<GameManager>().LevelClear();
    }
}
