using UnityEngine;
using Mirror;

public class PlayerThrow : NetworkBehaviour
{
    [SerializeField] private GameObject knifePrefab; // register in NetworkManager > Spawnable Prefabs
    [SerializeField] private Transform throwPoint;   // child at player hand
    [SerializeField] private float cooldown = 0.3f;

    private float nextTime;

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.F) && Time.time >= nextTime)
        {
            nextTime = Time.time + cooldown;

            // face-based direction (flip X scale when your player turns)
            Vector2 dir = transform.localScale.x <= 0 ? Vector2.right : Vector2.left;

            CmdThrowKnife(throwPoint.position, dir);
        }
    }

    [Command]
    void CmdThrowKnife(Vector3 spawnPos, Vector2 dir)
    {
        if (knifePrefab == null) return;

        var knifeGO = Instantiate(knifePrefab, spawnPos, Quaternion.identity);
        var knife = knifeGO.GetComponent<Knife>();
        knife.Initialize(dir);

        NetworkServer.Spawn(knifeGO);
    }
}