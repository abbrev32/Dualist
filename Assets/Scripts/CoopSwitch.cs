using UnityEngine;
using Mirror;
using System.Collections;

[RequireComponent(typeof(NetworkIdentity))]
public class CoopSwitch : NetworkBehaviour
{
    [SerializeField] private Elevator2 elevator;  // assign Elevator2
    [SerializeField] private float sinkDistance = 0.2f;
    [SerializeField] private float sinkSpeed = 1f;

    private Vector3 initialPosition;
    private Vector3 targetSinkPosition;
    private bool activated = false;

    private void Start()
    {
        initialPosition = transform.position;
        targetSinkPosition = initialPosition + Vector3.down * sinkDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || activated) return;

        if (isServer)
        {
            PlayerSteppedOn();
        }
        else
        {
            CmdPlayerSteppedOn();
        }
    }

    [Command]
    private void CmdPlayerSteppedOn(NetworkConnectionToClient sender = null)
    {
        PlayerSteppedOn();
    }

    private void PlayerSteppedOn()
    {
        if (activated) return;

        int playersOnSwitch = CountPlayersOnSwitch();

        if (playersOnSwitch >= 2)
        {
            activated = true;

            if (elevator != null)
                elevator.ActivateElevator();

            RpcSinkSwitch();
        }
    }

    private int CountPlayersOnSwitch()
    {
        Collider[] hits = Physics.OverlapBox(transform.position, GetComponent<Collider>().bounds.extents);
        int count = 0;
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
                count++;
        }
        return count;
    }

    [ClientRpc]
    private void RpcSinkSwitch()
    {
        StartCoroutine(SinkRoutine());
    }

    private IEnumerator SinkRoutine()
    {
        while (Vector3.Distance(transform.position, targetSinkPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetSinkPosition, sinkSpeed * Time.deltaTime);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
