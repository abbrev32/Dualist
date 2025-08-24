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

        // --- Input and State Calculation ---
        float moveX = Input.GetAxis("Horizontal");
        isRunning = Mathf.Abs(moveX) > 0.01f;
        isjumping = !IsOnGround();
        
        // --- Animator Booleans (works locally) ---
        if (netAnimator != null)
        {
            netAnimator.animator.SetBool("run", isRunning);
            netAnimator.animator.SetBool("grounded", IsOnGround());
        }

        // --- Handle Actions by sending Commands to Server ---
        HandleJumping();
        HandleDashing(moveX);
        HandleSwinging();
        HandleFlipping(moveX);

        // --- Physics (still controlled locally) ---
        // Note: For a server-authoritative game, you'd also send inputs to the server.
        // But for this animation issue, keeping physics client-side is fine.
        float finalVelocityX = CalculateFinalVelocityX(moveX);
        playerBody.linearVelocity = new Vector2(finalVelocityX, playerBody.linearVelocity.y);
    }
    
    // Extracted logic into separate methods for clarity
    #region Action Handlers

    private void HandleJumping()
    {
        float velocityY = playerBody.linearVelocity.y;
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);

        if (IsOnGround())
        {
            extJumps = 1;
            if (jumpPressed)
            {
                playerBody.linearVelocity = new Vector2(playerBody.linearVelocity.x, jumpHeight);
                CmdJump();
                PlayJumpSound();
            }
        }
        else if (extJumps > 0 && jumpPressed)
        {
            extJumps--;
            playerBody.linearVelocity = new Vector2(playerBody.linearVelocity.x, jumpHeight);
            CmdJump();
            PlayJumpSound();
        }
    }

    private void HandleDashing(float moveX)
    {
        if (isDashing) return;

        dashCoolDownTimer += Time.deltaTime;
        if (dashCoolDownTimer > dashCoolDown && Input.GetKeyDown(KeyCode.LeftShift))
        {
            CmdDash();
            if (audioSource != null && dashSound != null)
                audioSource.PlayOneShot(dashSound);
        }
    }

    private float CalculateFinalVelocityX(float moveX)
    {
        // This is a temporary state for dashing, we don't need a SyncVar
        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;
            float dashDirection = (moveX != 0) ? Mathf.Sign(moveX) : (transform.localScale.x > 0 ? 1 : -1);
            return (moveX * movementSpeed) + (dashSpeed * dashDirection);
        }
        return moveX * movementSpeed;
    }

    private void HandleSwinging()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (isjumping)
                CmdSwing("jump swing");
            else if (isRunning)
                CmdSwing("run swing");
            else if (IsOnGround())
                CmdSwing("idle swing");
        }
    }

    private void HandleFlipping(float moveX)
    {
        if (moveX != 0)
        {
            bool newFlipRight = moveX > 0;
            if (flipRight != newFlipRight)
                CmdSetFlip(newFlipRight);
        }
    }

    #endregion


    // --- Commands: Client -> Server ---

    [Command]
    private void CmdJump()
    {
        RpcJump(); // Server tells all clients to execute the RPC
    }

    [Command]
    private void CmdDash()
    {
        RpcDash(); // Server tells all clients to execute the RPC
    }

    [Command]
    private void CmdSwing(string triggerName)
    {
        RpcSwing(triggerName); // Server tells all clients to execute the RPC
    }

    [Command]
    private void CmdSetFlip(bool faceRight)
    {
        flipRight = faceRight; // SyncVar handles replication
    }

    
    // --- ClientRPCs: Server -> All Clients ---
    
    [ClientRpc]
    private void RpcJump()
    {
        // This code now runs on EVERY client
        netAnimator.SetTrigger("jump");
    }

    [ClientRpc]
    private void RpcDash()
    {
        // This code now runs on EVERY client
        // We also start the dash timer on all clients to keep physics consistent
        dashTimer = dashTime; 
        dashCoolDownTimer = 0f;
        netAnimator.SetTrigger("dash");
    }

    [ClientRpc]
    private void RpcSwing(string triggerName)
    {
        // This code now runs on EVERY client
        netAnimator.SetTrigger(triggerName);
    }


    // --- SyncVar Hook ---

    private void OnFlipChanged(bool oldValue, bool newValue)
    {
        Vector3 scale = transform.localScale;
        // Flipped the logic here to be more intuitive: positive scale.x should mean facing right.
        scale.x = newValue ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    // --- Helpers ---

    private void PlayJumpSound()
    {
        if (audioSource != null && jumpSound != null)
            audioSource.PlayOneShot(jumpSound);
    }

    private bool IsOnGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.8f, LayerMask.GetMask("Platform"));
        return hit.collider != null;
    }
}