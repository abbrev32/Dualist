using UnityEngine;

// This script should be attached to the player or sword object.
// It requires an AudioSource component on the same GameObject.
[RequireComponent(typeof(AudioSource))]
public class SwordMouseRotate : MonoBehaviour
{
    // The Audio Clip to be played when the sword is swung.
    // Assign this directly in the Unity Inspector.
    public AudioClip swordSwingSound;

    public float rotationSpeed = 5f; // Adjust to your liking
    public Transform swordPivot; // The empty pivot object

    // The minimum horizontal mouse movement required to trigger the sword sound.
    public float swordSwingThreshold = 0.5f;

    // A reference to the AudioSource component.
    private AudioSource audioSource;
    // A variable to track the mouse's total horizontal movement for rotation.
    private float currentAngle = 0f;

    // Cooldown variables to control sound frequency.
    // Adjust this value to change how often the sound can play.
    public float soundCooldown = 0.2f;
    private float lastSoundTime = 0f;

    void Awake()
    {
        // Get the AudioSource component attached to this GameObject.
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Get mouse horizontal movement
        float mouseX = Input.GetAxis("Mouse X");

        // Check if there is significant horizontal mouse movement and the cooldown has passed.
        if (Mathf.Abs(mouseX) > swordSwingThreshold && Time.time > lastSoundTime + soundCooldown)
        {
            // Play the sound effect using the directly assigned audio clip.
            if (audioSource != null && swordSwingSound != null)
            {
                audioSource.PlayOneShot(swordSwingSound);
                // Update the last sound time to start the cooldown.
                lastSoundTime = Time.time;
            }
        }

        // Update the rotation based on mouse movement.
        currentAngle += mouseX * rotationSpeed;

        // Rotate pivot based on mouse movement.
        swordPivot.localRotation = Quaternion.Euler(0f, 0f, currentAngle);
    }
}
