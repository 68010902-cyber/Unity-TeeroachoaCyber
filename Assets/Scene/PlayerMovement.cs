
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("ความเร็วเคลื่อนที่")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 8f;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Dash - ระบบพุ่งแนวนอน")]
    [SerializeField] private float dashSpeed = 1000f;
    [SerializeField] private float dashDuration = 0.25f;
    [SerializeField] private float dashCooldown = 0.8f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isGrounded;
    private bool isDashing;
    private float nextDashTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();

        if (rb == null)
        {
            Debug.LogError("ไม่พบ Rigidbody2D บนตัวละคร");
            enabled = false;
            return;
        }

        rb.gravityScale = 1f;
        rb.freezeRotation = true;

        if (groundLayer.value == 0)
            groundLayer = LayerMask.GetMask("Ground");
    }

    private void Update()
    {
        if (rb == null) return;

        isGrounded = Physics2D.OverlapCircle(
            (Vector2)transform.position + Vector2.down * 0.6f,
            groundCheckRadius,
            groundLayer
        );

        float moveInput = 0f;
        bool jumpPressed = false;
        bool dashLeft = false;
        bool dashRight = false;

        if (Keyboard.current != null)
        {
            var keyboard = Keyboard.current;

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                moveInput = -1f;

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                moveInput = 1f;

            jumpPressed =
                keyboard.spaceKey.wasPressedThisFrame ||
                keyboard.wKey.wasPressedThisFrame ||
                keyboard.upArrowKey.wasPressedThisFrame;

            bool shift =
                keyboard.leftShiftKey.isPressed ||
                keyboard.rightShiftKey.isPressed;

            dashLeft = shift &&
                (keyboard.aKey.wasPressedThisFrame ||
                 keyboard.leftArrowKey.wasPressedThisFrame);

            dashRight = shift &&
                (keyboard.dKey.wasPressedThisFrame ||
                 keyboard.rightArrowKey.wasPressedThisFrame);
        }

        if (!isDashing)
        {
            if (dashLeft && Time.time >= nextDashTime)
                StartCoroutine(Dash(-1));

            else if (dashRight && Time.time >= nextDashTime)
                StartCoroutine(Dash(1));

            if (jumpPressed && isGrounded)
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            if (!isDashing)
                rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }

        if (spriteRenderer != null && moveInput != 0)
            spriteRenderer.flipX = moveInput < 0;

        if (animator != null && !isDashing)
        {
            animator.SetBool("IsRunning", Mathf.Abs(moveInput) > 0.01f);
            animator.SetBool("IsJumping", !isGrounded);
        }
    }

    private IEnumerator Dash(int direction)
    {
        isDashing = true;
        nextDashTime = Time.time + dashCooldown;

        // ล็อกการเคลื่อนที่ในแนวตั้งชั่วคราว
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(direction * dashSpeed, 0f);

        if (spriteRenderer != null)
            spriteRenderer.flipX = direction < 0;

        Debug.Log("Dash direction: " + direction +
                  " | Speed: " + dashSpeed);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        isDashing = false;
    }
}