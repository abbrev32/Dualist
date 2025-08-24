using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    [Header("Movement Settings")]
    public Rigidbody2D playerBody;
    public float movementSpeed = 5f;
    public float dashSpeed = 15f;
    public float jumpHeight = 5f;

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
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private readonly float dashTime = 0.25f;
    private readonly float dashCooldown = 1f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        netAnimator = GetComponent<NetworkAnimator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    public override void OnStartLocalPlayer()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            CameraBehavior camScript = mainCam.GetComponent<CameraBehavior>();
            if (camScript != null) camScript.playerPos = transform;
        }
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        HandleMovementInput();
        HandleDashInput();
        HandleAttackInput();
        UpdateAnimatorStates();
    }

    private void HandleMovementInput()
    {
        float moveX = Input.GetAxis("Horizontal");
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);

        if (IsOnGround())
        {
            extJumps = 1;
            if (jumpPressed)
            {
                CmdJump();
            }
        }
        else if (!IsOnGround() && extJumps > 0 && jumpPressed)
        {
            extJumps--;
            CmdJump();
        }

        // Flip
        if (moveX != 0)
        {
            bool newFlipRight = moveX > 0;
            if (flipRight != newFlipRight)
                CmdSetFlip(newFlipRight);
        }

        // Apply horizontal movement (without dash)
        if (!isDashing)
            CmdMove(moveX * movementSpeed);
    }

    private void HandleDashInput()
    {
        dashCooldownTimer += Time.deltaTime;

        if (!isDashing && dashCooldownTimer >= dashCooldown && Input.GetKeyDown(KeyCode.LeftShift))
        {
            isDashing = true;
            dashCooldownTimer = 0f;
            if (audioSource != null && dashSound != null)
                audioSource.PlayOneShot(dashSound);

            float dashDirection = Input.GetAxisRaw("Horizontal");
            if (dashDirection == 0) dashDirection = flipRight ? 1 : -1;
            CmdDash(dashDirection);
        }

        if (isDashing)
        {
            dashTimer += Time.deltaTime;
            if (dashTimer >= dashTime)
            {
                isDashing = false;
                dashTimer = 0f;
            }
        }
    }

    private void HandleAttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            bool isRunning = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01f;
            bool isJumping = !IsOnGround();

            if (netAnimator != null)
            {
                if (isRunning) netAnimator.SetTrigger("run swing");
                else if (isJumping) netAnimator.SetTrigger("jump swing");
                else netAnimator.SetTrigger("idle swing");
            }
        }
    }

    private void UpdateAnimatorStates()
    {
        float moveX = Input.GetAxis("Horizontal");
        bool isRunning = Mathf.Abs(moveX) > 0.01f;
        bool grounded = IsOnGround();

        if (netAnimator != null)
        {
            netAnimator.animator.SetBool("run", isRunning);
            netAnimator.animator.SetBool("grounded", grounded);
        }
    }

    #region Commands

    [Command]
    private void CmdMove(float velocityX)
    {
        if (!isDashing)
        {
            Vector2 velocity = playerBody.linearVelocity;
            velocity.x = velocityX;
            playerBody.linearVelocity = velocity;
        }
    }

    [Command]
    private void CmdJump()
    {
        Vector2 velocity = playerBody.linearVelocity;
        velocity.y = jumpHeight;
        playerBody.linearVelocity = velocity;

        if (netAnimator != null) netAnimator.SetTrigger("jump");
        PlayJumpSound();
    }

    [Command]
    private void CmdDash(float direction)
    {
        Vector2 velocity = playerBody.linearVelocity;
        velocity.x = direction * dashSpeed;
        playerBody.linearVelocity = velocity;

        if (netAnimator != null) netAnimator.SetTrigger("dash");
    }

    [Command]
    private void CmdSetFlip(bool faceRight)
    {
        flipRight = faceRight;
    }

    #endregion

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
