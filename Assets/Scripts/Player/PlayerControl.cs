using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    [Header("Movement Settings")]
    public Rigidbody2D playerBody;
    public float movementSpeed = 1.0f;
    public float dashSpeed = 15.0f;
    public float jumpHeight = 5.0f;

    [Header("Audio")]
    public AudioClip jumpSound;
    public AudioClip dashSound;
    private AudioSource audioSource;

    [Header("Components")]
    private Animator anim;
    private NetworkAnimator netAnimator;

    [SyncVar(hook = nameof(OnFlipChanged))]
    private bool flipRight = true;

    private int extJumps = 0;
    private bool isDashing = false;
    private readonly float dashCoolDown = 1f;
    private float dashCoolDownTimer = 0f;
    private float dashTimer = 0f;
    private readonly float dashTime = 0.25f;

    [SyncVar]
    public bool isRunning = false;
    [SyncVar]
    public bool isjumping = false;
    [SyncVar]
    public bool onGround = true;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        netAnimator = GetComponent<NetworkAnimator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public override void OnStartLocalPlayer()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            CameraBehavior camScript = mainCam.GetComponent<CameraBehavior>();
            if (camScript != null)
                camScript.playerPos = transform;
        }
    }

    private void Update()
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
                netAnimator.SetTrigger("jump");
                PlayJumpSound();
            }
        }
        else if (!IsOnGround() && extJumps > 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocityY = jumpHeight;
                extJumps--;
                netAnimator.SetTrigger("jump");
                PlayJumpSound();
            }
        }

        float finalVelocityX = moveX * movementSpeed;
        isRunning = Mathf.Abs(moveX) > 0.01f;
        isjumping = !IsOnGround();
        // Dash
        if (!isDashing)
        {
            dashCoolDownTimer += Time.deltaTime;
            if (dashCoolDownTimer > dashCoolDown && Input.GetKeyDown(KeyCode.LeftShift))
            {
                isDashing = true;
                if (audioSource != null && dashSound != null)
                    audioSource.PlayOneShot(dashSound);
            }
        }
        else
        {
            dashTimer += Time.deltaTime;
            if (dashTimer > dashTime)
            {
                isDashing = false;
                dashCoolDownTimer = 0f;
                dashTimer = 0f;
            }
            else
            {
                float dashDirection = (moveX != 0) ? moveX : transform.localScale.x > 0 ? 1 : -1;
                finalVelocityX += dashSpeed * dashDirection;
                netAnimator.SetTrigger("dash");
            }
        }

        // Apply movement
        playerBody.linearVelocity = new Vector2(finalVelocityX, velocityY);

        onGround = IsOnGround();
        // Flip
        if (moveX != 0)
        {
            bool newFlipRight = moveX > 0;
            if (flipRight != newFlipRight)
                CmdSetFlip(newFlipRight);
        }

        // Animator states
        if (netAnimator != null)
        {
            netAnimator.animator.SetBool("run", isRunning);
            netAnimator.animator.SetBool("grounded", onGround);
        }
        // Swing sword
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Only pick one swing trigger per click
            if (isjumping)
            {
                CmdSwing("jump swing");
            }
            else if (isRunning)
            {
                CmdSwing("run swing");
            }
            else // grounded and not running
            {
                CmdSwing("idle swing");
            }
        }

    }
    [Command]
    private void CmdSwing(string triggerName)
    {
        // On the server, apply the trigger to the animator
        netAnimator.animator.SetTrigger(triggerName);
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

    private void PlayJumpSound()
    {
        if (audioSource != null && jumpSound != null)
            audioSource.PlayOneShot(jumpSound);
    }

    private bool IsOnGround()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float length = 0.8f;

        LayerMask groundLayer = LayerMask.GetMask("Platform");
        RaycastHit2D hit = Physics2D.Raycast(position, direction, length, groundLayer);

        return hit.collider != null;
    }
}