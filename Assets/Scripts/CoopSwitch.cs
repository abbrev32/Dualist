using UnityEngine;
using Mirror;

public class CoopSwitch : NetworkBehaviour
{
    [SerializeField] private Elevator2 elevator;

    private int playersOnSwitch = 0;
    private bool activated = false;

    // Called when a player enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playersOnSwitch++;
        Debug.Log("Player stepped on switch: " + other.name + " | Players on switch: " + playersOnSwitch);

        if (!activated && playersOnSwitch >= 2)  // Change 2 to how many players needed
        {
            activated = true;
            Debug.Log("Switch activated!");
            if (elevator != null)
                elevator.ActivateElevator();

            // Optional: sink the switch visually
            StartCoroutine(SinkSwitchRoutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playersOnSwitch--;
        Debug.Log("Player left switch: " + other.name + " | Players on switch: " + playersOnSwitch);
    }

    private System.Collections.IEnumerator SinkSwitchRoutine()
    {
        Transform visual = transform.GetChild(0); // assuming first child is the visual
        Vector3 startPos = visual.localPosition;
        Vector3 endPos = startPos + Vector3.down * 0.5f; // sink 0.5 units
        float t = 0f;
        float duration = 1.5f;

        while (t < duration)
        {
            t += Time.deltaTime;
            visual.localPosition = Vector3.Lerp(startPos, endPos, t / duration);
            yield return null;
        }

        visual.localPosition = endPos;
        Debug.Log("Switch sunk and finished.");
    }
}
