using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ArrowWeapon : MonoBehaviour
{
    // Inspector: References
    [Header("References")]
    [Tooltip("Assigned by WeaponManager when the weapon is spawned.")]
    public Transform player;

    [Tooltip("Optional tip transform used as the aim origin (auto-find child named 'Tip').")]
    public Transform tip;

    // Inspector: Targeting
    [Header("Targeting")]
    public LayerMask enemyMask;
    public float scanRadius = 4f;

    [Tooltip("If true, only target enemies visible by the main camera.")]
    public bool onlyTargetOnScreen = true;

    [Tooltip("If true, after hitting an enemy we avoid targeting that same enemy for the next shot (if possible).")]
    public bool avoidLastTarget = true;

    // Inspector: Shooting
    [Header("Shooting")]
    public float fireCooldown = 0.8f;
    public float lockTime = 0.15f;
    public float projectileSpeed = 12f;
    public float maxLifeTime = 2.5f;
    public int damage = 10;

    // Inspector: Sprite direction
    [Header("Sprite Direction")]
    [Tooltip("Angle offset applied to sprite rotation. Set 180 if your sprite points LEFT by default.")]
    public float spriteAngleOffset = 0f;

    // Cached components
    private Rigidbody2D rb;
    private Collider2D col;
    private Camera cam;

    // Timers
    private float cooldownTimer;  // time since last shot (or since start)
    private float lockTimer;      // time spent locking before firing
    private float lifeTimer;      // time spent flying before snapping back

    // Targeting state
    private Transform currentTarget;
    private Transform lastTarget; // last enemy hit (optional avoidance)
    private Vector2 shootDir;

    // Orbit cache: WeaponManager places weapons around the player as children.
    // When shooting we detach from parent; when done we reattach and restore local position.
    private Vector3 orbitLocalPos;
    private Transform orbitParent;

    // Behaviour state machine
    private enum State
    {
        Hover,      // orbiting around player, waiting for cooldown
        Locking,    // found target, brief lock to aim (feels more intentional)
        Shooting    // flying forward as projectile
    }

    private State state = State.Hover;

    // Unity lifecycle
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // Orbit behaviour:
        // - Kinematic + not simulated while orbiting so we don't fight parent transform.
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.freezeRotation = true;
        rb.simulated = false;

        // Collider is only enabled while shooting to avoid hitting enemies while orbiting.
        col.isTrigger = true;
        col.enabled = false;

        cam = Camera.main;

        // Start ready to fire (makes testing easier).
        cooldownTimer = fireCooldown;
    }

    private void Start()
    {
        // Auto-find tip if not assigned
        if (tip == null)
        {
            Transform found = transform.Find("Tip");
            if (found != null) tip = found;
        }

        // Cache the orbit parent + local position. WeaponManager may move localPosition later,
        // so we refresh this cache during Hover/Locking.
        orbitParent = transform.parent;
        orbitLocalPos = transform.localPosition;
    }

    private void Update()
    {
        if (player == null) return;

        // Keep orbit cache updated (WeaponManager can reposition our localPosition).
        // Only do this when we're not flying.
        if (state == State.Hover || state == State.Locking)
        {
            orbitParent = transform.parent;
            orbitLocalPos = transform.localPosition;
        }

        // Cooldown timer counts up every frame.
        cooldownTimer += Time.deltaTime;

        // State machine
        switch (state)
        {
            case State.Hover:
                // We are orbiting: try to acquire a target once cooldown is ready.
                TryStartLock();
                break;

            case State.Locking:
                // Brief lock-on time: rotate to face target, then fire.
                UpdateLocking();
                break;

            case State.Shooting:
                // If we didn't hit anything, return back after maxLifeTime.
                lifeTimer += Time.deltaTime;
                if (lifeTimer >= maxLifeTime)
                    SnapBackToOrbit();
                break;
        }
    }

    private void FixedUpdate()
    {
        // Rigidbody movement should happen in FixedUpdate for stable physics.
        if (state == State.Shooting)
        {
            rb.MovePosition(rb.position + shootDir * projectileSpeed * Time.fixedDeltaTime);
        }
    }

    // Targeting & shooting
    private void TryStartLock()
    {
        // Wait for cooldown
        if (cooldownTimer < fireCooldown) return;

        // Find a target within scan radius
        currentTarget = FindNearestEnemy();
        if (currentTarget == null) return;

        // Start lock phase
        lockTimer = 0f;
        state = State.Locking;
    }

    private void UpdateLocking()
    {
        // If target got destroyed or disappeared, cancel lock.
        if (currentTarget == null)
        {
            state = State.Hover;
            return;
        }

        // Keep aiming at target while locking
        lockTimer += Time.deltaTime;

        Vector2 origin = GetAimOrigin();
        Vector2 toTarget = (Vector2)currentTarget.position - origin;

        // Rotate sprite to face target (visual feedback)
        RotateToDirection(toTarget);

        // After lockTime, fire toward the target position.
        if (lockTimer >= lockTime)
            FireAtTarget(currentTarget);
    }

    private void FireAtTarget(Transform target)
    {
        Vector2 origin = GetAimOrigin();
        Vector2 toTarget = (Vector2)target.position - origin;

        // Prevent invalid direction
        if (toTarget.sqrMagnitude < 0.0001f)
        {
            state = State.Hover;
            return;
        }

        shootDir = toTarget.normalized;

        // Detach from orbit while flying (so WeaponManager doesn't drag it back).
        transform.SetParent(null, true);

        // Enable physics simulation and trigger collider for hits.
        rb.simulated = true;
        col.enabled = true;

        // Align sprite to travel direction
        RotateToDirection(shootDir);

        // Reset timers
        lifeTimer = 0f;
        cooldownTimer = 0f;

        state = State.Shooting;
    }

    private void SnapBackToOrbit()
    {
        // Disable projectile behaviour
        col.enabled = false;
        rb.simulated = false;

        // Reattach to orbit and restore local position
        if (orbitParent != null)
        {
            transform.SetParent(orbitParent, true);
            transform.localPosition = orbitLocalPos;
        }

        // Clear current target
        currentTarget = null;

        state = State.Hover;
    }

    // Collision & damage
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only apply damage during flight
        if (state != State.Shooting) return;

        // EnemyLife is the enemy health script used in this project.
        // GetComponentInParent allows collider to be on a child object.
        EnemyLife enemy = other.GetComponentInParent<EnemyLife>();
        if (enemy == null) return;

        // Apply damage
        enemy.TakeDamage(damage);

        // Track last target so we can avoid instantly re-targeting it (optional).
        lastTarget = enemy.transform;

        // Return to orbit immediately on hit.
        SnapBackToOrbit();
    }

    // Internal helpers
    private Transform FindNearestEnemy()
    {
        // Scan around the aim origin
        Collider2D[] hits = Physics2D.OverlapCircleAll(GetAimOrigin(), scanRadius, enemyMask);
        if (hits == null || hits.Length == 0) return null;

        Transform best = null;
        float bestDist = float.MaxValue;

        // First pass: try to avoid lastTarget (if enabled)
        foreach (Collider2D hit in hits)
        {
            Transform t = hit.transform;

            if (avoidLastTarget && lastTarget != null && t == lastTarget) continue;
            if (onlyTargetOnScreen && !IsOnScreen(t.position)) continue;

            // Use player position for distance (consistent target choice)
            float d = (t.position - player.position).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = t;
            }
        }

        // Optional fallback: if all were excluded because of lastTarget, allow it.
        if (best == null && avoidLastTarget && lastTarget != null)
        {
            foreach (Collider2D hit in hits)
            {
                Transform t = hit.transform;

                if (onlyTargetOnScreen && !IsOnScreen(t.position)) continue;

                float d = (t.position - player.position).sqrMagnitude;
                if (d < bestDist)
                {
                    bestDist = d;
                    best = t;
                }
            }
        }

        return best;
    }

    private Vector2 GetAimOrigin()
    {
        // Prefer tip for better aiming (front of arrow), else player, else self
        if (tip != null) return tip.position;
        if (player != null) return player.position;
        return transform.position;
    }

    private void RotateToDirection(Vector2 dir)
    {
        // No rotation for near-zero direction
        if (dir.sqrMagnitude < 0.0001f) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + spriteAngleOffset);
    }

    private bool IsOnScreen(Vector3 worldPos)
    {
        // If camera is missing, do not block targeting.
        if (cam == null) cam = Camera.main;
        if (cam == null) return true;

        // Viewport coordinates: (0..1) is on screen.
        Vector3 v = cam.WorldToViewportPoint(worldPos);
        return v.z > 0f && v.x >= 0f && v.x <= 1f && v.y >= 0f && v.y <= 1f;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Visualise scan radius in editor
        Gizmos.DrawWireSphere(GetAimOrigin(), scanRadius);
    }
#endif
}
