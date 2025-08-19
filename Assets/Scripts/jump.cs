using UnityEngine;

// This script should be attached to the player GameObject.
// It requires an AudioSource component on the same GameObject.
[RequireComponent(typeof(AudioSource))]
public class PlayerJumpSound : MonoBehaviour
{
    // The Audio Clip to be played when the player jumps.
    // Assign this in the Unity Inspector.
    public AudioClip jumpSound;

    // A reference to the AudioSource component.
    private AudioSource audioSource;

    // The name of the jump animation trigger or a boolean to check for jumping.
    // Replace with your actual animation parameter name if you're using one.
    private const string jumpTriggerName = "Jump";

    // A reference to the Animator component (if you are using animations).
    private Animator playerAnimator;

    void Awake()
    {
        // Get the AudioSource component attached to this GameObject.
        audioSource = GetComponent<AudioSource>();

        // Set the AudioSource's clip to the jumpSound.
        // This is a good practice to ensure the correct clip is always loaded.
        if (jumpSound != null)
        {
            audioSource.clip = jumpSound;
        }

        // Get the Animator component if it exists.
        // This is optional and only needed if you are using an animation trigger for jumping.
        playerAnimator = GetComponent<Animator>();
    }

    // This method can be called from your player controller script.
    // For example, in the part of your code that handles the jump input.
    public void PlayJumpSound()
    {
        // Check if a jump sound clip has been assigned and the AudioSource is not null.
        if (jumpSound != null && audioSource != null)
        {
            // Play the sound effect once.
            audioSource.PlayOneShot(jumpSound);
        }
        else
        {
            Debug.LogWarning("Jump sound or AudioSource is missing! Please assign them in the Inspector.");
        }
    }

    // You can also use this method if you're triggering the sound via an animation event.
    // Create an Animation Event on the jump animation clip and call this function.
    public void OnAnimationJump()
    {
        PlayJumpSound();
    }

    // An example of how you might call the sound from an Update method.
    // This is a simple example and might not be suitable for all games.
    // A better approach is to call PlayJumpSound() from your main player movement script.
    /*
    void Update()
    {
        // Example: If the player presses the space bar and is on the ground.
        // You would replace `Input.GetKeyDown(KeyCode.Space)` with your actual jump condition.
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            // This would be the code in your player controller script.
            // After triggering the jump, call PlayJumpSound.
            // Rigidbody.velocity.y = jumpForce;
            // PlayJumpSound();
        }
    }

    private bool IsGrounded()
    {
        // Implement your ground check logic here.
        // For example, a Raycast or sphere check.
        return true;
    }
    */
}

