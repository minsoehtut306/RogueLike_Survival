using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GoblinChase : MonoBehaviour
{
    [Header("Chase")]
    public float moveSpeed = 2.2f;
    public float stopDistance = 0.2f;

    [Header("References")]
    public Transform target;     // Player
    public Animator animator;

    private Rigidbody2D rb;

    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");
    private static readonly int FacingHash = Animator.StringToHash("Facing");
    private static readonly int IsHurtHash = Animator.StringToHash("IsHurt"); // optional

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (target == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) target = p.transform;
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // Optional: if enemy is hurt, stop moving so hurt isn't overridden visually
        if (animator != null && animator.GetBool(IsHurtHash))
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 toTarget = (Vector2)target.position - rb.position;
        float dist = toTarget.magnitude;

        Vector2 dir = dist > 0.001f ? (toTarget / dist) : Vector2.zero;
        Vector2 velocity = (dist <= stopDistance) ? Vector2.zero : dir * moveSpeed;

        rb.linearVelocity = velocity;

        if (animator != null)
        {
            // MoveX/MoveY for Blend Tree (recommended)
            Vector2 moveDir = velocity.sqrMagnitude > 0.0001f ? velocity.normalized : Vector2.zero;
            animator.SetFloat(MoveXHash, moveDir.x);
            animator.SetFloat(MoveYHash, moveDir.y);

            // Facing: 0 front, 1 back, 2 left, 3 right
            if (moveDir.sqrMagnitude > 0.0001f)
            {
                int facing;
                if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
                    facing = moveDir.x > 0 ? 3 : 2;
                else
                    facing = moveDir.y > 0 ? 1 : 0;

                animator.SetInteger(FacingHash, facing);
            }
        }
    }
}
