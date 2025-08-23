using UnityEngine;
using Mirror;
using System;

// This script controls player movement and networking.
public class PlayerController : NetworkBehaviour
{
    // A reference to the Rigidbody2D component for physics-based movement.
    public Rigidbody2D playerBody;
    // Public variables to control movement and jump properties in the Inspector.
    public float movementSpeed = 1.0f;
    public float dashSpeed = 15.0f;
    public float jumpHeight = 5.0f;

    // Public variable for the jump sound.
    public AudioClip jumpSound;

    // Public variable for the dash sound clip.
    public AudioClip dashSound;

    // The audio source component.
    private AudioSource audioSource;

    private int extJumps = 0;

    // References for animations.
    private Animator anim;
    private NetworkAnimator netAnimator;

    // A networked variable to sync the player's flip direction.
    //[SyncVar(hook = nameof(OnFlipChanged))]
    //private float flipX = 0;
    [SyncVar(hook = nameof(OnFlipChanged))]
    private bool flipRight = true;

    // Dash forward variables.
    private bool isDashing = false;
    private readonly float dashCoolDown = 1;
    float dashCoolDownTimer = 0;
    float dashTimer = 0;
    private readonly float dashTime = 0.25f;

    private void Awake()
    {
        // Get references to all necessary components on this GameObject.
        anim = GetComponent<Animator>();
        netAnimator = GetComponent<NetworkAnimator>();


        // Get or add the AudioSource component.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

    }
    public override void OnStartLocalPlayer()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            CameraBehavior camScript = mainCam.GetComponent<CameraBehavior>();
            if (camScript != null)
            {
                camScript.playerPos = transform;   // assign THIS player to camera
            }
        }
    }


    void Update()
    {
        // Only allow the local player to control the character.
        if (!isLocalPlayer) return;

        // Linear Movement + Double Jumping logic.
        float moveX = Input.GetAxis("Horizontal");
        float velocityY = playerBody.linearVelocity.y; // Correctly get y velocity.

        // Jump + double jump logic.
        if (IsOnGround())
        {
            extJumps = 1;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocityY = jumpHeight;
                //CmdSetAnimTrigger("jump");
                // Play the jump sound locally for the jumping player.
                PlayJumpSound();
            }
        }
        // On-air jump logic.
        else if (!IsOnGround() && extJumps > 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocityY = jumpHeight;
                extJumps--;
                //CmdSetAnimTrigger("jump");
                // Play the double jump sound locally.
                PlayJumpSound();
            }
        }

        // Add final velocity for clarity.
        float finalVelocityX = moveX * movementSpeed;
        bool isRunning = Math.Abs(moveX) > 0.01f;

        // Dash logic.
        if (!isDashing)
        {
            dashCoolDownTimer += Time.deltaTime;
            if (dashCoolDownTimer > dashCoolDown)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    isDashing = true;

                    // Play the dash sound here.
                    if (audioSource != null && dashSound != null)
                    {
                        audioSource.PlayOneShot(dashSound);
                    }
                }
            }
        }
        else
        {
            dashTimer += Time.deltaTime;
            if (dashTimer > dashTime)
            {
                isDashing = false;
                dashCoolDownTimer = 0;
                dashTimer = 0;
            }
            else
            {
                // Dash to last faced direction if not moving.
                float dashDirection = (moveX != 0) ? moveX : transform.localScale.x > 0 ? 1 : -1;
                // Add dash to final velocity.
                finalVelocityX += dashSpeed * dashDirection;
            }
        }

        // Assign velocities.
        playerBody.linearVelocity = new Vector2(finalVelocityX, velocityY);

        // Handle player facing direction.
        //float newFlipX = 0;
        //if (moveX != 0)
        //{
        //    newFlipX = moveX > 0 ? 0.2f : -0.2f;
        //}
        //else
        //{
        //    newFlipX = transform.localScale.x > 0 ? 0.2f : -0.2f;
        //}
        //if (newFlipX != flipX)
        //{
        //    CmdSetFlip(newFlipX);
        //}
        if (moveX != 0)
        {
            flipRight = moveX > 0;
        }
        //else
        //{
        //    flipRight = transform.localScale.x > 0;
        //}

        if (netAnimator != null)
        {
            CmdSetAnimationState("run", isRunning);
            CmdSetAnimationState("grounded", IsOnGround());
        }
    }

    [Command]
    public void CmdSetAnimationState(string param, bool value)
    {
        netAnimator.animator.SetBool(param, value);
    }

    [Command]
    void CmdSetAnimTrigger(string param)
    {
        netAnimator.SetTrigger(param);
    }

    //[Command]
    //void CmdSetFlip(float value)
    //{
    //    flipX = value;
    //}
    //void OnFlipChanged(float oldValue, float newValue)
    //{
    //    transform.localScale = new Vector3(newValue, 0.2f, 0.2f);
    //}
    void OnFlipChanged(bool oldValue, bool newValue)
    {
        if (oldValue != newValue)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    // Method to play the jump sound.
    private void PlayJumpSound()
    {
        if (audioSource != null && jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

    bool IsOnGround()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float length = 0.8f;

        LayerMask groundLayer = LayerMask.GetMask("Platform");

        RaycastHit2D hit = Physics2D.Raycast(position, direction, length, groundLayer);

        return (hit.collider != null);
    }
}

