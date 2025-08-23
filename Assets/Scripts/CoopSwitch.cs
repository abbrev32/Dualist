using UnityEngine;
using Mirror;
using System.Collections;

public class CoopSwitch : NetworkBehaviour
{
    [SerializeField] private Elevator2 elevator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip switchActivationSound;
    [SerializeField] private AudioClip[] switchActivationMusicTracks;

    [Header("Turrets")]
    [SerializeField] private GameObject turretPrefab1; 
    [SerializeField] private GameObject turretPrefab2; 
    [SerializeField] private Transform turretSpawnPoint1;
    [SerializeField] private Transform turretSpawnPoint2;

    private int playersOnSwitch = 0;
    private bool activated = false;
    private int currentMusicIndex = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playersOnSwitch++;

        if (!activated && playersOnSwitch >= 2)
        {
            activated = true;
            Debug.Log("Switch activated!");

            if (isServer) // ✅ Only the server can spawn turrets
            {
                SpawnTurrets();
            }

            BackgroundMusic musicManager = FindObjectOfType<BackgroundMusic>();
            if (musicManager != null)
                musicManager.StopMusic();

            if (elevator != null)
                elevator.ActivateElevator();

            if (audioSource != null && switchActivationSound != null && switchActivationMusicTracks.Length > 0)
            {
                audioSource.PlayOneShot(switchActivationSound);
                StartCoroutine(PlayMusicAfterDelay(switchActivationSound.length));
            }

            StartCoroutine(SinkSwitchRoutine());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playersOnSwitch--;
    }

    private IEnumerator PlayMusicAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(PlayMusicTracks());
    }

    private IEnumerator PlayMusicTracks()
    {
        while (true)
        {
            if (audioSource != null && switchActivationMusicTracks.Length > 0)
            {
                audioSource.clip = switchActivationMusicTracks[currentMusicIndex];
                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length);
                currentMusicIndex = (currentMusicIndex + 1) % switchActivationMusicTracks.Length;
            }
            else break;
        }
    }

    private IEnumerator SinkSwitchRoutine()
    {
        Transform visual = transform.GetChild(0);
        Vector3 startPos = visual.localPosition;
        Vector3 endPos = startPos + Vector3.down * 0.5f;
        float t = 0f;
        float duration = 1.5f;

        while (t < duration)
        {
            t += Time.deltaTime;
            visual.localPosition = Vector3.Lerp(startPos, endPos, t / duration);
            yield return null;
        }

        visual.localPosition = endPos;
    }

    // ✅ Turret spawning happens only on the server
    [Server]
    private void SpawnTurrets()
    {
        if (turretPrefab1 != null && turretSpawnPoint1 != null)
        {
            GameObject turret1 = Instantiate(turretPrefab1, turretSpawnPoint1.position, Quaternion.identity);
            NetworkServer.Spawn(turret1);
            Debug.Log("Turret 1 spawned!");
        }

        if (turretPrefab2 != null && turretSpawnPoint2 != null)
        {
            GameObject turret2 = Instantiate(turretPrefab2, turretSpawnPoint2.position, Quaternion.identity);
            NetworkServer.Spawn(turret2);
            Debug.Log("Turret 2 spawned!");
        }
    }
}
