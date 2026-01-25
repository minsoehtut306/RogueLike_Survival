using System.Collections;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    // Inspector: Health
    [Header("Health")]
    [Tooltip("Maximum health of the enemy.")]
    public int maxHP = 30;

    private int currentHP;

    // Inspector: Hurt behaviour
    [Header("Hurt")]
    [Tooltip("How long the enemy is locked in a hurt state after being hit.")]
    public float hurtLockTime = 0.15f;

    // Inspector: Death behaviour
    [Header("Death")]
    [Tooltip("Fallback destroy time if death animation event is missing.")]
    public float deathFallbackDestroyTime = 1.2f;

    // Cached components
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;
    private GoblinChase chase;
    private ContactDamage contactDamage;

    // State
    private Coroutine hurtRoutine;
    private bool isDead;

    // Animator hashes
    private static readonly int HurtHash = Animator.StringToHash("Hurt");
    private static readonly int DieHash = Animator.StringToHash("Die");
    private static readonly int IsHurtHash = Animator.StringToHash("IsHurt");

    // Unity lifecycle
    private void Awake()
    {
        currentHP = maxHP;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        chase = GetComponent<GoblinChase>();
        contactDamage = GetComponent<ContactDamage>();
    }

    // Public API
    public void TakeDamage(int damage)
    {
        // Ignore damage if already dead
        if (isDead) return;

        currentHP -= damage;

        // If this hit kills the enemy, skip hurt logic and die immediately
        if (currentHP <= 0)
        {
            Die();
            return;
        }

        // Trigger hurt feedback if animator exists
        if (animator != null)
        {
            animator.ResetTrigger(DieHash); // safety reset
            animator.SetTrigger(HurtHash);

            // Restart hurt lock coroutine
            if (hurtRoutine != null)
                StopCoroutine(hurtRoutine);

            hurtRoutine = StartCoroutine(HurtLock());
        }
    }

    // Internal behaviour
    private IEnumerator HurtLock()
    {
        // Mark enemy as hurt so movement scripts can pause
        if (animator != null)
            animator.SetBool(IsHurtHash, true);

        // Stop movement and chasing while hurt
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (chase != null)
            chase.enabled = false;

        yield return new WaitForSeconds(hurtLockTime);

        // Restore movement if still alive
        if (!isDead && chase != null)
            chase.enabled = true;

        if (animator != null)
            animator.SetBool(IsHurtHash, false);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Stop any hurt behaviour
        if (hurtRoutine != null)
            StopCoroutine(hurtRoutine);

        if (animator != null)
            animator.SetBool(IsHurtHash, false);

        // Disable combat and movement interactions
        if (chase != null)
            chase.enabled = false;

        if (contactDamage != null)
            contactDamage.enabled = false;

        if (col != null)
            col.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        // Trigger death animation (animation event should call OnDeathAnimationComplete)
        if (animator != null)
            animator.SetTrigger(DieHash);

        // Safety fallback in case animation event is missing
        Invoke(nameof(OnDeathAnimationComplete), deathFallbackDestroyTime);
    }

    // Animation Event
    public void OnDeathAnimationComplete()
    {
        // Cancel fallback invoke if animation event fires first
        CancelInvoke(nameof(OnDeathAnimationComplete));

        Destroy(gameObject);
    }
}
