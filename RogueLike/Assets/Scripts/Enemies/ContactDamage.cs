using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ContactDamage : MonoBehaviour
{
    // Inspector: Damage settings
    [Header("Damage")]
    [Tooltip("Damage dealt to the player per hit.")]
    public int damage = 10;

    [Tooltip("Minimum time between damage ticks while staying in contact.")]
    public float damageInterval = 0.5f;

    // Cooldown timer (prevents damage every physics frame)
    private float timer;

    // Unity lifecycle
    private void Update()
    {
        // Count down timer in normal Update (frame independent)
        if (timer > 0f)
            timer -= Time.deltaTime;
    }

    // Collision callbacks
    private void OnCollisionStay2D(Collision2D collision)
    {
        TryDamage(collision.collider);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamage(other);
    }

    // Internal helpers
    private void TryDamage(Collider2D other)
    {
        // Respect interval
        if (timer > 0f) return;

        // Allow Player tag on child collider OR parent object
        bool isPlayer =
            other.CompareTag("Player") ||
            (other.transform.parent != null && other.transform.parent.CompareTag("Player"));

        if (!isPlayer) return;

        // PlayerHealth might be on the root player object (not on the collider object)
        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null) return;

        // Apply damage and reset timer
        playerHealth.TakeDamage(damage);
        timer = damageInterval;
    }
}
