using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    // Inspector: Health
    [Header("Health")]
    public int maxHP = 10;
    public int currentHP;

    // Inspector: Invincibility
    [Header("Invincibility")]
    [Tooltip("Seconds of invincibility after taking damage.")]
    public float iFrameSeconds = 0.6f;

    private float iFrameTimer;

    // Inspector: Death animation
    [Header("Death Animation")]
    [Tooltip("Animator used to play the death animation.")]
    public Animator animator;

    [Tooltip("Bool parameter used to enter the Die state.")]
    public string dieBoolName = "Die";

    // Inspector: Events
    [Header("Events")]
    [Tooltip("Called when the player dies (Game Over UI, sounds, etc).")]
    public UnityEvent onDeath;

    // Runtime state
    public bool IsDead { get; private set; }

    // Cached references (disabled on death)
    private PlayerMovement playerMovement;
    private WeaponManager weaponManager;
    private Rigidbody2D rb;
    private Collider2D[] colliders;

    // Unity lifecycle
    private void Awake()
    {
        currentHP = maxHP;

        playerMovement = GetComponent<PlayerMovement>();
        weaponManager = GetComponent<WeaponManager>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponentsInChildren<Collider2D>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Countdown invincibility timer
        if (iFrameTimer > 0f)
            iFrameTimer -= Time.deltaTime;
    }

    // Public API
    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        if (iFrameTimer > 0f) return;

        currentHP -= amount;
        iFrameTimer = iFrameSeconds;

        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        currentHP = Mathf.Min(maxHP, currentHP + amount);
    }

    // Internal helpers
    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        // Disable player control
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Disable weapons so nothing fires after death
        if (weaponManager != null)
            weaponManager.enabled = false;

        // Stop physics movement
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // Disable all colliders so enemies stop dealing contact damage
        if (colliders != null)
        {
            foreach (var c in colliders)
                c.enabled = false;
        }

        // Trigger death animation
        if (animator != null)
            animator.SetBool(dieBoolName, true);

        // Notify UI / GameManager
        onDeath?.Invoke();
    }
}
