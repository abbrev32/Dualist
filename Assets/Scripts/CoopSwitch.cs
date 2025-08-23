using UnityEngine;
using Mirror;

public class CoopSwitch : NetworkBehaviour
{
    // A reference to the elevator script.
    [SerializeField] private Elevator2 elevator;
    // A reference to the AudioSource component that will play the music.
    [SerializeField] private AudioSource backgroundMusicSource;
    // The sound file to play when the switch is activated.
    [SerializeField] private AudioClip switchActivationMusic;

    // Tracks the number of players on the switch.
    private int playersOnSwitch = 0;
    // A flag to ensure the switch activates only once.
    private bool activated = false;

    // Called when a player enters the trigger collider.
    private void OnTriggerEnter2D(UnityEngine.Collider2D other)
    {
        // Check if the entering object is a player.
        if (!other.CompareTag("Player")) return;

        // Increment the player count and log the event.
        playersOnSwitch++;
        Debug.Log("Player stepped on switch: " + other.name + " | Players on switch: " + playersOnSwitch);

        // Check if the switch is not yet activated and enough players are on it.
        if (!activated && playersOnSwitch >= 2) // Change 2 to how many players are needed
        {
            // Set the activated flag to true to prevent re-activation.
            activated = true;
            Debug.Log("Switch activated!");

            // If the elevator reference is not null, activate it.
            if (elevator != null)
                elevator.ActivateElevator();

            // Play the background music if the references are set.
            if (backgroundMusicSource != null && switchActivationMusic != null)
            {
                backgroundMusicSource.clip = switchActivationMusic;
                backgroundMusicSource.Play();
            }

            // Start the visual routine for the switch.
            StartCoroutine(SinkSwitchRoutine());
        }
    }

    // Called when a player exits the trigger collider.
    private void OnTriggerExit2D(UnityEngine.Collider2D other)
    {
        // Check if the exiting object is a player.
        if (!other.CompareTag("Player")) return;

        // Decrement the player count and log the event.
        playersOnSwitch--;
        Debug.Log("Player left switch: " + other.name + " | Players on switch: " + playersOnSwitch);
    }

    // A coroutine to visually "sink" the switch over time.
    private System.Collections.IEnumerator SinkSwitchRoutine()
    {
        Transform visual = transform.GetChild(0); // Assuming the first child is the visual part of the switch.
        Vector3 startPos = visual.localPosition;
        Vector3 endPos = startPos + Vector3.down * 0.5f; // Sinks the switch by 0.5 units.
        float t = 0f;
        float duration = 1.5f;

        while (t < duration)
        {
            t += Time.deltaTime;
            visual.localPosition = Vector3.Lerp(startPos, endPos, t / duration);
            yield return null;
        }

        // Ensure the switch ends at the final position.
        visual.localPosition = endPos;
        Debug.Log("Switch sunk and finished.");
    }
}
