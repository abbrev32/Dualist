using UnityEngine;
using Mirror;

public class CoopSwitch : NetworkBehaviour
{
    [SerializeField] private Elevator2 elevator;  // Reference to Elevator2
    [SerializeField] private float sinkSpeed = 1f;

    private int playersOnSwitch = 0;
    private bool activated = false;

    private Vector3 initialPosition;
    private Vector3 sunkPosition;

    private void Start()
    {
        initialPosition = transform.position;
        sunkPosition = initialPosition + Vector3.down * 0.2f; // how far it sinks
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return; // only server checks
        if (other.CompareTag("Player"))
        {
            playersOnSwitch++;
            CheckActivation();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isServer) return;
        if (other.CompareTag("Player"))
        {
            playersOnSwitch--;
        }
    }

    [Server]
    private void CheckActivation()
    {
        if (playersOnSwitch >= 2 && !activated)
        {
            activated = true;

            // Only server tells elevator to activate
            if (elevator != null)
            {
                elevator.ActivateElevator();
            }

            RpcSinkSwitch();
        }
    }

    [ClientRpc]
    private void RpcSinkSwitch()
    {
        StartCoroutine(SinkRoutine());
    }

    private System.Collections.IEnumerator SinkRoutine()
    {
        while (Vector3.Distance(transform.position, sunkPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, sunkPosition, sinkSpeed * Time.deltaTime);
            yield return null;
        }
        // switch disappears into ground
        gameObject.SetActive(false);
    }
}
