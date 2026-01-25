using UnityEngine;

public class TargetArrowWorld : MonoBehaviour
{
    // Inspector: References
    [Header("References")]
    [Tooltip("Player transform. Arrow will rotate from the player position.")]
    public Transform player;

    [Tooltip("Manager that provides the closest collectible target.")]
    public TargetCollectibleManager targetManager;

    // Inspector: Behaviour
    [Header("Behaviour")]
    [Tooltip("Offset from player where the arrow should appear.")]
    public Vector2 arrowOffset = new Vector2(0.7f, 0.3f);

    [Tooltip("Angle offset applied to arrow sprite (set if sprite points up/left by default).")]
    public float spriteAngleOffset = 0f;

    [Tooltip("Hide arrow if no target exists.")]
    public bool hideWhenNoTarget = true;

    // Unity lifecycle
    private void Awake()
    {
        // Auto-find player if not assigned
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // Auto-find manager if not assigned
        if (targetManager == null)
            targetManager = TargetCollectibleManager.Instance;
    }

    private void LateUpdate()
    {
        if (player == null) return;

        if (targetManager == null)
            targetManager = TargetCollectibleManager.Instance;

        // Find closest target
        Transform target = (targetManager != null)
            ? targetManager.GetCurrentTargetTransform(player.position)
            : null;

        // Hide arrow if no target
        if (target == null)
        {
            if (hideWhenNoTarget && gameObject.activeSelf)
                gameObject.SetActive(false);

            return;
        }

        // Ensure arrow is visible once a target exists again
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        // Position arrow near player
        transform.position = player.position + new Vector3(arrowOffset.x, arrowOffset.y, 0f);

        // Rotate arrow toward target
        Vector2 dir = (Vector2)target.position - (Vector2)transform.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + spriteAngleOffset);
    }
}
