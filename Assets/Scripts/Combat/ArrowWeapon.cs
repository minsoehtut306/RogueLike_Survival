using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ArrowWeapon : MonoBehaviour
{
    [Header("References")]
    public Transform player;     // assigned by WeaponManager when spawned
    public Transform tip;

    [Header("Targeting")]
    public LayerMask enemyMask;
    public float scanRadius = 4f;
    public bool onlyTargetOnScreen = true;
    public bool avoidLastTarget = true;

    [Header("Shooting")]
    public float fireCooldown = 0.8f;
    public float lockTime = 0.15f;
    public float projectileSpeed = 12f;
    public float maxLifeTime = 2.5f;
    public int damage = 10;

    [Header("Sprite Direction")]
    public float spriteAngleOffset = 0f; // set 180 if sprite points LEFT

    Rigidbody2D rb;
    Collider2D col;

    float cooldownTimer;
    float lockTimer;
    float lifeTimer;

    Transform currentTarget;
    Transform lastTarget;
    Vector2 shootDir;

    Camera cam;

    // We store where WeaponManager placed us (local position in the orbit)
    Vector3 orbitLocalPos;
    Transform orbitParent;

    enum State { Hover, Locking, Shooting }
    State state = State.Hover;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.freezeRotation = true;

        col.isTrigger = true;
        col.enabled = false;

        cooldownTimer = fireCooldown;
        cam = Camera.main;

        // IMPORTANT: default to not simulated while orbiting
        rb.simulated = false;
    }

    void Start()
    {
        if (tip == null)
        {
            Transform t = transform.Find("Tip");
            if (t != null) tip = t;
        }

        orbitParent = transform.parent;
        orbitLocalPos = transform.localPosition;
    }

    void Update()
    {
        if (player == null) return;

        // keep orbit cache updated (WeaponManager moves our localPosition)
        if (state == State.Hover || state == State.Locking)
        {
            orbitParent = transform.parent;
            orbitLocalPos = transform.localPosition;
        }

        cooldownTimer += Time.deltaTime;

        switch (state)
        {
            case State.Hover:
                TryStartLock();
                break;
            case State.Locking:
                LockingUpdate();
                break;
            case State.Shooting:
                lifeTimer += Time.deltaTime;
                if (lifeTimer >= maxLifeTime)
                    SnapBackToOrbit();
                break;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        if (state == State.Shooting)
        {
            // physics-driven movement only while shooting
            rb.MovePosition(rb.position + shootDir * projectileSpeed * Time.fixedDeltaTime);
        }
        // Hover/Locking: do nothing. Transform follows parent naturally (rb.simulated=false).
    }

    // -------------------- Targeting --------------------
    void TryStartLock()
    {
        if (cooldownTimer < fireCooldown) return;

        currentTarget = FindNearestEnemy();
        if (currentTarget == null) return;

        lockTimer = 0f;
        state = State.Locking;
    }

    void LockingUpdate()
    {
        if (currentTarget == null)
        {
            state = State.Hover;
            return;
        }

        Vector2 origin = GetAimOrigin();
        Vector2 toTarget = (Vector2)currentTarget.position - origin;

        if (toTarget.sqrMagnitude > 0.0001f)
        {
            shootDir = toTarget.normalized;
            RotateTo(shootDir);
        }

        lockTimer += Time.deltaTime;
        if (lockTimer >= lockTime)
            FireNow();
    }

    void FireNow()
    {
        // Detach so it doesn't get dragged by player movement
        transform.SetParent(null, true);

        rb.simulated = true;
        col.enabled = true;

        state = State.Shooting;
        cooldownTimer = 0f;
        lifeTimer = 0f;
    }

    // -------------------- Hit / Reset --------------------
    void OnTriggerEnter2D(Collider2D other)
    {
        if (state != State.Shooting) return;

        if (((1 << other.gameObject.layer) & enemyMask) == 0)
            return;

        EnemyLife enemy = other.GetComponent<EnemyLife>();
        if (enemy != null)
            enemy.TakeDamage(damage);

        lastTarget = other.transform;
        SnapBackToOrbit();
    }

    void SnapBackToOrbit()
    {
        currentTarget = null;

        state = State.Hover;
        lifeTimer = 0f;

        col.enabled = false;
        rb.simulated = false;

        // Re-attach back to the player orbit
        transform.SetParent(player, true);
        transform.localPosition = orbitLocalPos;

        transform.rotation = Quaternion.identity;
    }

    // -------------------- Helpers --------------------
    Vector2 GetAimOrigin()
    {
        if (tip != null) return tip.position;
        return transform.position;
    }

    Transform FindNearestEnemy()
    {
        Vector2 origin = GetAimOrigin();
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, scanRadius, enemyMask);
        if (hits == null || hits.Length == 0) return null;

        Transform best = null;
        float bestDist = float.PositiveInfinity;

        foreach (var h in hits)
        {
            if (h == null) continue;
            if (avoidLastTarget && h.transform == lastTarget) continue;
            if (onlyTargetOnScreen && !IsOnScreen(h.transform.position)) continue;

            float d = ((Vector2)h.transform.position - origin).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = h.transform;
            }
        }

        if (best == null)
        {
            foreach (var h in hits)
            {
                if (h == null) continue;
                if (onlyTargetOnScreen && !IsOnScreen(h.transform.position)) continue;

                float d = ((Vector2)h.transform.position - origin).sqrMagnitude;
                if (d < bestDist)
                {
                    bestDist = d;
                    best = h.transform;
                }
            }
        }

        return best;
    }

    bool IsOnScreen(Vector3 worldPos)
    {
        if (cam == null) return true;
        Vector3 vp = cam.WorldToViewportPoint(worldPos);
        return vp.z > 0 && vp.x >= 0 && vp.x <= 1 && vp.y >= 0 && vp.y <= 1;
    }

    void RotateTo(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + spriteAngleOffset;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnDrawGizmosSelected()
    {
        Vector3 origin = (tip != null) ? tip.position : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, scanRadius);
    }
}
