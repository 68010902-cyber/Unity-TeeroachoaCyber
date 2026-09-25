using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("ความเร็วเคลื่อนที่")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 8f;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();

        if (rb == null)
        {
            Debug.LogError("PlayerMovement ต้องใช้ Rigidbody2D บน GameObject ตัวละคร");
            return;
        }

        rb.gravityScale = 1f;
        rb.freezeRotation = true;
        rb.velocity = Vector2.zero;

        if (groundLayer.value == 0)
            groundLayer = LayerMask.GetMask("Ground");

        if (animator != null)
        {
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsJumping", false);
        }
    }

    private void Update()
    {
        if (rb == null)
            return;

        isGrounded = Physics2D.OverlapCircle((Vector2)transform.position + Vector2.down * 0.6f, groundCheckRadius, groundLayer);

        float moveInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                moveInput = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                moveInput = 1f;
        }
        else
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                moveInput = -1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                moveInput = 1f;
        }

        bool jumpPressed = false;

        if (Keyboard.current != null)
        {
            jumpPressed = Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame;
        }
        else
        {
            jumpPressed = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        }

        if (jumpPressed && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        bool isRunning = Mathf.Abs(moveInput) > 0.01f;
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (spriteRenderer != null)
            spriteRenderer.flipX = moveInput < 0f;

        if (animator != null)
        {
            animator.SetBool("IsRunning", isRunning);
            animator.SetBool("IsJumping", !isGrounded);
        }
    }
}

