using UnityEngine;
using Mirror;

public class Lever : NetworkBehaviour
{
    public Elevator elevator; // assign in Inspector
    public AudioSource audioSource; // assign in Inspector
    public AudioClip leverSound; // assign in Inspector
    private SpriteRenderer spriteRenderer;

    [SyncVar(hook = nameof(OnFlipChanged))]
    private bool isOn = false; // false = OFF, true = ON

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateFlipVisual();
    }

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (elevator == null) return;

        // Play sound effect on server
        if (audioSource != null && leverSound != null)
            audioSource.PlayOneShot(leverSound);

        if (other.CompareTag("Player") && !isOn)
        {
            // Player can only turn lever ON
            isOn = true;
            elevator.MoveToBottom();

            // Enable collider again
            PolygonCollider2D col = elevator.GetComponent<PolygonCollider2D>();
            if (col != null)
                col.enabled = true;
        }
        else if (other.CompareTag("Monster") && isOn)
        {
            // Monster can only turn lever OFF
            isOn = false;
            elevator.MoveToTop();

            // Disable collider
            PolygonCollider2D col = elevator.GetComponent<PolygonCollider2D>();
            if (col != null)
                col.enabled = false;
        }
    }

    // Called on all clients when isOn changes
    void OnFlipChanged(bool oldValue, bool newValue)
    {
        UpdateFlipVisual();
    }

    private void UpdateFlipVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = isOn; // ON = flipped, OFF = normal
        }
    }
}
