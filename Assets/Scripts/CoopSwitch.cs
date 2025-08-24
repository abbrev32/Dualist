using UnityEngine;
using Mirror;
using System.Collections;

public class CoopSwitch : NetworkBehaviour
{
    // A reference to the elevator script.
    [SerializeField] private Elevator2 elevator;
    // The AudioSource component that will play the music and sound effects.
    [SerializeField] private AudioSource audioSource;
    // The sound file to play when the switch is activated.
    [SerializeField] private AudioClip switchActivationSound;
    // The main background music tracks to play after the switch is activated.
    [SerializeField] private AudioClip[] switchActivationMusicTracks;

    // Tracks the number of players on the switch.
    private int playersOnSwitch = 0;
    // A flag to ensure the switch activates only once.
    private bool activated = false;
    // Index to keep track of the current music track.
    private int currentMusicIndex = 0;

    // Called when a player enters the trigger collider.
    private void OnTriggerEnter2D(UnityEngine.Collider2D other)
    {
        // Check if the entering object is a player.
        if (!other.CompareTag("Player")) return;

        // Increment the player count and log the event.
        playersOnSwitch++;
        Debug.Log("Player stepped on switch: " + other.name + " | Players on switch: " + playersOnSwitch);

        // Check if the switch is not yet activated and enough players are on it.
        if (!activated && playersOnSwitch >= 1) // Change 2 to how many players are needed
        {
            // Set the activated flag to true to prevent re-activation.
            activated = true;
            Debug.Log("Switch activated!");

            // Find the BackgroundMusic script in the scene and stop its music.
            BackgroundMusic musicManager = FindObjectOfType<BackgroundMusic>();
            if (musicManager != null)
            {
                musicManager.StopMusic();
            }

            // If the elevator reference is not null, activate it.
            if (elevator != null)
                elevator.ActivateElevator();

            // Play the switch activation sound effect and then the music.
            if (audioSource != null && switchActivationSound != null && switchActivationMusicTracks.Length > 0)
            {
                audioSource.PlayOneShot(switchActivationSound);
                StartCoroutine(PlayMusicAfterDelay(switchActivationSound.length));
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

    // A coroutine to play the main music after a delay and then loop through the rest.
    private IEnumerator PlayMusicAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Start the music playing coroutine.
        StartCoroutine(PlayMusicTracks());
    }

    private IEnumerator PlayMusicTracks()
    {
        while (true) // This loop will continue indefinitely, cycling through the tracks.
        {
            if (audioSource != null && switchActivationMusicTracks.Length > 0)
            {
                // Set the audio clip to the current track.
                audioSource.clip = switchActivationMusicTracks[currentMusicIndex];

                // Play the music.
                audioSource.Play();

                // Wait for the current track to finish playing.
                yield return new WaitForSeconds(audioSource.clip.length);

                // Move to the next track in the array.
                currentMusicIndex = (currentMusicIndex + 1) % switchActivationMusicTracks.Length;
            }
            else
            {
                // Break the loop if there are no tracks to play.
                break;
            }
        }
    }

    // A coroutine to visually "sink" the switch over time.
    private IEnumerator SinkSwitchRoutine()
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