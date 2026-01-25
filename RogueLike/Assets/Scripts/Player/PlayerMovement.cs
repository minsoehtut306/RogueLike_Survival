using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    // Inspector: Movement
    [Header("Movement")]
    public float moveSpeed = 5f;

    // Inspector: Input
    [Header("Joystick (DynamicJoystick)")]
    [Tooltip("Assign the DynamicJoystick component (TouchZone).")]
    public DynamicJoystick joystick;

    // Inspector: Input filtering
    [Header("Input Filtering")]
    [Tooltip("Joystick input below this is ignored for movement (prevents drift and idle flicker).")]
    public float deadzone = 0.15f;

    [Tooltip("Extra small hold time so animation doesn't instantly drop to idle on 1-frame joystick jitter.")]
    public float idleDropDelay = 0.08f;

    // Cached components
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerHealth health;

    // Runtime state
    private Vector2 moveInput;           // filtered move input
    private Vector2 lastDir = Vector2.down;

    private float lastNonZeroTime;       // used to keep Speed non-zero briefly

    // Unity lifecycle
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        health = GetComponent<PlayerHealth>();

        // Stable 2D movement setup
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void Update()
    {
        // If dead: stop animation and movement input
        if (health != null && health.IsDead)
        {
            if (anim != null)
                anim.SetFloat("Speed", 0f);

            moveInput = Vector2.zero;
            return;
        }

        // 1) Read joystick input (raw)
        Vector2 raw = (joystick != null) ? joystick.Input : Vector2.zero;

        // 2) Apply deadzone (movement + animation stability)
        if (raw.magnitude < deadzone)
            raw = Vector2.zero;

        moveInput = raw;

        // 3) Update last direction only when actually moving
        if (moveInput.sqrMagnitude > 0.001f)
        {
            lastDir = moveInput.normalized;
            lastNonZeroTime = Time.time;
        }

        // 4) Animator parameters
        if (anim != null)
        {
            // Keep last facing direction for idle
            anim.SetFloat("MoveX", lastDir.x);
            anim.SetFloat("MoveY", lastDir.y);

            // Speed is based on filtered input magnitude (0..1 for joystick)
            float speed = moveInput.magnitude;

            // If joystick jitters to zero for a frame, keep speed briefly
            if (speed <= 0.0001f && (Time.time - lastNonZeroTime) < idleDropDelay)
                speed = deadzone; // small non-zero to prevent transition to idle

            anim.SetFloat("Speed", speed);
        }
    }

    private void FixedUpdate()
    {
        // If dead: do not move
        if (health != null && health.IsDead) return;

        // Prevent faster diagonal movement
        Vector2 dir = (moveInput.sqrMagnitude > 0.001f) ? moveInput.normalized : Vector2.zero;

        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
    }
}
