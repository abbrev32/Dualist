using UnityEngine;
using Mirror;
using System;
using Mirror.BouncyCastle.Security;
public class PlayerController : NetworkBehaviour
{
    public Rigidbody2D playerBody;
    public float movementSpeed = 1.0f;
    public float dashSpeed = 15.0f;
    public float jumpHeight = 5.0f;
    private int extJumps = 0;

    private Animator anim;
    [SyncVar]
    private bool grounded = true;
    [SyncVar]
    private bool isRunning = false;
    [SyncVar]

    //Dash forward
    private bool isDashing = false;
    private readonly float dashCoolDown = 1;
    float dashCoolDownTimer = 0;
    float dashTimer = 0;
    private readonly float dashTime = 0.25f;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            //Linear Movement + Double Jumping
            float moveX = Input.GetAxis("Horizontal");
            float velocityY = playerBody.linearVelocityY;

            //jump + double jump logic
            if (IsOnGround())
            {
                extJumps = 1;
                grounded = true;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    velocityY = jumpHeight;
                    grounded = false;
                    CmdJump();
                }
            }
            //on-air jump
            if (!IsOnGround() && extJumps > 0)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    velocityY = jumpHeight;
                    extJumps--;
                    grounded = false;
                    CmdJump();
                }
            }
            //add final velocity for clearity
            float finalVelocityX = moveX * movementSpeed;


            //turning character
            float inputMovValue = Input.GetAxis("Horizontal");

            if (inputMovValue > 0.01f)
                transform.localScale = new Vector3(.2f, 0.2f, 0.2f);
            else if (inputMovValue < -0.01f)
                transform.localScale = new Vector3(-0.2f, 0.2f, 0.2f);
            //set animinator parameter
            isRunning = (inputMovValue != 0);
            anim.SetBool("run", isRunning);



            //dash logic... long ahh and boring but i made it by myself :D
            if (!isDashing)
            {
                dashCoolDownTimer += Time.deltaTime;
                if (dashCoolDownTimer > dashCoolDown)
                {
                    if (Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        isDashing = true;
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
                    //dash to last faced direction if not moving
                    float dashDirection = (moveX != 0) ? moveX : transform.localScale.x > 0 ? 1 : -1;
                    //add dash to final velocity
                    finalVelocityX += dashSpeed * dashDirection;
                }
            }


            //Assign velocities
            playerBody.linearVelocity = new Vector2(finalVelocityX, velocityY);
        }
        if (anim != null)
        {
            anim.SetBool("run", isRunning);
            anim.SetBool("grounded", grounded);
        }
    }

    [Command]
    void CmdJump()
    {
        RpcTriggerJump();
    }
    [ClientRpc]
    void RpcTriggerJump()
    {
        if (anim != null) anim.SetTrigger("jump");
    }
    bool IsOnGround()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float length = 0.5f;

        LayerMask groundLayer = LayerMask.GetMask("Platform");

        RaycastHit2D hit = Physics2D.Raycast(position, direction, length, groundLayer);

        return (hit.collider != null);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            grounded = true;
    }
}