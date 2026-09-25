using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7.5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (groundCheck == null)
            groundCheck = transform.Find("GroundCheck");
    }

    private void Update()
    {
        float moveInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                moveInput = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                moveInput = 1f;
        }

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = moveInput < 0f;
        }

        var groundPosition = groundCheck != null ? groundCheck.position : transform.position;
        isGrounded = groundLayer.value == 0 || Physics2D.OverlapCircle(groundPosition, groundCheckRadius, groundLayer);

        bool jumpPressed = Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame);

        if (jumpPressed && isGrounded && rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        UpdateAnimation(moveInput);
    }

    private void UpdateAnimation(float moveInput)
    {
        if (animator == null)
            return;

        bool isMoving = Mathf.Abs(moveInput) > 0.01f;

        animator.SetBool("IsRunning", isMoving && isGrounded);
        animator.SetBool("IsJumping", !isGrounded);
        animator.SetBool("IsGrounded", isGrounded);

        if (!isGrounded)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
                animator.Play("Jump");
            return;
        }

        if (isMoving)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
                animator.Play("Run");
            return;
        }

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            animator.Play("Idle");
    }
}