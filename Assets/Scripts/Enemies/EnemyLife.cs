using System.Collections;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 30;
    private int hp;

    [Header("Hurt")]
    public float hurtLockTime = 0.15f;

    [Header("Death")]
    public float deathFallbackDestroyTime = 1.2f; // safety if event isn't added yet

    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;
    private GoblinChase chase;
    private ContactDamage contactDamage;

    private Coroutine hurtRoutine;
    private bool dead;

    private static readonly int HurtHash = Animator.StringToHash("Hurt");
    private static readonly int DieHash = Animator.StringToHash("Die");
    private static readonly int IsHurtHash = Animator.StringToHash("IsHurt");

    void Awake()
    {
        hp = maxHP;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        chase = GetComponent<GoblinChase>();
        contactDamage = GetComponent<ContactDamage>();
    }

    public void TakeDamage(int dmg)
    {
        if (dead) return;

        hp -= dmg;

        // If this hit kills the enemy: DIE ONLY (no hurt)
        if (hp <= 0)
        {
            Die();
            return;
        }

        // Otherwise play hurt
        if (animator != null)
        {
            animator.ResetTrigger("Die");     // safety
            animator.SetTrigger("Hurt");

            if (hurtRoutine != null) StopCoroutine(hurtRoutine);
            hurtRoutine = StartCoroutine(HurtLock());
        }
    }

    private IEnumerator HurtLock()
    {
        if (animator != null) animator.SetBool(IsHurtHash, true);

        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (chase != null) chase.enabled = false;

        yield return new WaitForSeconds(hurtLockTime);

        if (!dead && chase != null) chase.enabled = true;
        if (animator != null) animator.SetBool(IsHurtHash, false);
    }

    private void Die()
    {
        if (dead) return;
        dead = true;

        // Stop hurt routine and clear flag
        if (hurtRoutine != null) StopCoroutine(hurtRoutine);
        if (animator != null) animator.SetBool(IsHurtHash, false);

        // Stop movement + disable interactions
        if (chase != null) chase.enabled = false;
        if (contactDamage != null) contactDamage.enabled = false;
        if (col != null) col.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        // Trigger death animation
        if (animator != null)
            animator.SetTrigger(DieHash);

        // Fallback: if you forget to add animation event, still remove after delay
        Invoke(nameof(OnDeathAnimationComplete), deathFallbackDestroyTime);
    }

    /// <summary>
    /// Called by Animation Event on last frame of Goblin_Die.anim
    /// </summary>
    public void OnDeathAnimationComplete()
    {
        // Cancel fallback invoke if event fires first (safe)
        CancelInvoke(nameof(OnDeathAnimationComplete));
        Destroy(gameObject);
    }
}
