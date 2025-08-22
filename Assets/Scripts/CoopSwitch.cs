using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
public class CoopSwitch : NetworkBehaviour
{
    [SerializeField] private Elevator2 elevator;  // Drag Elevator2 here in Inspector
    [SerializeField] private float sinkDistance = 0.2f; // how far switch sinks
    [SerializeField] private float sinkSpeed = 1f;      // speed of sinking

    private int playersOnSwitch = 0;
    private bool activated = false;

    private Vector3 initialPosition;
    private Vector3 targetSinkPosition;

    private void Start()
    {
        initialPosition = transform.position;
        targetSinkPosition = initialPosition + Vector3.down * sinkDistance;
    }

    [ServerCallback] // Only server counts players
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || activated) return;

        playersOnSwitch++;
        CheckActivation();
    }

    [ServerCallback]
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || activated) return;

        playersOnSwitch = Mathf.Max(playersOnSwitch - 1, 0);
    }

    [Server]
    private void CheckActivation()
    {
        Debug.Log($"Players on switch: {playersOnSwitch}");

        if (playersOnSwitch >= 2 && !activated)
        {
            Debug.Log("Switch activated!");
            activated = true;

            if (elevator != null)
                elevator.ActivateElevator();

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
        while (Vector3.Distance(transform.position, targetSinkPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetSinkPosition, sinkSpeed * Time.deltaTime);
            yield return null;
        }
        gameObject.SetActive(false); // disappear after sinking
    }
}
