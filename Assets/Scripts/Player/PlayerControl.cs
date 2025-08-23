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
        bool isRunning = Mathf.Abs(moveX) > 0.01f;
        bool isjumping = !IsOnGround();
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
            }
        }

        // Apply movement
        playerBody.linearVelocity = new Vector2(finalVelocityX, velocityY);

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
            netAnimator.animator.SetBool("grounded", IsOnGround());
        }

        // Swing sword
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (netAnimator != null)
            {
                if (isRunning)
                    netAnimator.SetTrigger("run swing");
                if (isjumping)
                    netAnimator.SetTrigger("jump swing");
                if(!isRunning && IsOnGround())
                    netAnimator.SetTrigger("idle swing");


            }
        }
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
