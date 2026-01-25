using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GoblinChase : MonoBehaviour
{
    // Inspector: Chase settings
    [Header("Chase")]
    [Tooltip("Movement speed while chasing the player.")]
    public float moveSpeed = 2.2f;

    [Tooltip("Stop moving when closer than this distance.")]
    public float stopDistance = 0.2f;

    // Inspector: References
    [Header("References")]
    [Tooltip("Target to chase (usually the Player). Auto-finds by tag if null.")]
    public Transform target;

    [Tooltip("Animator for movement direction parameters (optional).")]
    public Animator animator;

    // Cached components
    private Rigidbody2D rb;

    // Animator hashes (faster + safer than string calls every frame)
    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");
    private static readonly int FacingHash = Animator.StringToHash("Facing");
    private static readonly int IsHurtHash = Animator.StringToHash("IsHurt"); // used by EnemyLife (optional)

    // Unity lifecycle
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Auto-find player if not assigned
        if (target == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) target = p.transform;
        }
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        // If EnemyLife is currently playing a hurt lock, stop moving so hurt visuals aren't overridden.
        if (animator != null && animator.GetBool(IsHurtHash))
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Direction toward target
        Vector2 toTarget = (Vector2)target.position - rb.position;
        float dist = toTarget.magnitude;

        // Normalised direction (safe for very small values)
        Vector2 dir = (dist > 0.001f) ? (toTarget / dist) : Vector2.zero;

        // Stop when close enough
        Vector2 velocity = (dist <= stopDistance) ? Vector2.zero : dir * moveSpeed;
        rb.linearVelocity = velocity;

        // Update animator movement parameters (if animator exists)
        if (animator != null)
        {
            // Use velocity direction if moving, else keep previous direction (optional)
            Vector2 moveDir = (velocity.sqrMagnitude > 0.0001f) ? velocity.normalized : Vector2.zero;

            animator.SetFloat(MoveXHash, moveDir.x);
            animator.SetFloat(MoveYHash, moveDir.y);

            // Facing: 0 front, 1 back, 2 left, 3 right (only update if moving)
            if (moveDir.sqrMagnitude > 0.0001f)
            {
                int facing;

                if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
                    facing = (moveDir.x > 0) ? 3 : 2;
                else
                    facing = (moveDir.y > 0) ? 1 : 0;

                animator.SetInteger(FacingHash, facing);
            }
        }
    }
}
