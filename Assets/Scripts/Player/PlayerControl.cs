using UnityEngine;
using Mirror;
using System;

public class PlayerController : NetworkBehaviour
{
    public Rigidbody2D playerBody;
    public float movementSpeed = 1.0f;
    public float dashSpeed = 15.0f;
    public float jumpHeight = 5.0f;

    public AudioClip jumpSound;
    public AudioClip dashSound;

    private AudioSource audioSource;

    private int extJumps = 0;

    private Animator anim;
    private NetworkAnimator netAnimator;

    [SyncVar(hook = nameof(OnFlipChanged))]
    private bool flipRight = true;

    private bool isDashing = false;
    private readonly float dashCoolDown = 1;
    float dashCoolDownTimer = 0;
    float dashTimer = 0;
    private readonly float dashTime = 0.25f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        netAnimator = GetComponent<NetworkAnimator>();

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
                camScript.playerPos = transform;
            }
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        float moveX = Input.GetAxis("Horizontal");
        float velocityY = playerBody.linearVelocity.y;

        // Jump + double jump
        if (IsOnGround())
        {
            extJumps = 1;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocityY = jumpHeight;
                CmdSetAnimTrigger("jump");
                PlayJumpSound();
            }
        }
        else if (!IsOnGround() && extJumps > 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocityY = jumpHeight;
                extJumps--;
                PlayJumpSound();
            }
        }

        float finalVelocityX = moveX * movementSpeed;
        bool isRunning = Math.Abs(moveX) > 0.01f;

        // Dashing
        if (!isDashing)
        {
            dashCoolDownTimer += Time.deltaTime;
            if (dashCoolDownTimer > dashCoolDown)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    isDashing = true;
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
                float dashDirection = (moveX != 0) ? moveX : transform.localScale.x > 0 ? 1 : -1;
                finalVelocityX += dashSpeed * dashDirection;
            }
        }

        // Assign velocity
        playerBody.linearVelocity = new Vector2(finalVelocityX, velocityY);

        // Flip handling (only send command when direction changes)
        if (moveX != 0)
        {
            bool newFlipRight = moveX > 0;
            if (flipRight != newFlipRight)
            {
                CmdSetFlip(newFlipRight);
            }
        }

        // Sync animator states
        if (netAnimator != null)
        {
            CmdSetAnimationState("run", isRunning);
            CmdSetAnimationState("grounded", IsOnGround());
        }
        
        //swing sword
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (netAnimator != null)
            {
                netAnimator.SetTrigger("idle swing");
            }
        }
    }

    // ===== Commands and hooks =====

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

    [Command]
    private void CmdSetFlip(bool faceRight)
    {
        flipRight = faceRight;
    }

    private void OnFlipChanged(bool oldValue, bool newValue)
    {
        Vector3 scale = transform.localScale;
        scale.x = newValue ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    // ===== Helpers =====

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
