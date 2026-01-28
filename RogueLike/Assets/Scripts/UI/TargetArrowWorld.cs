using UnityEngine;

public class TargetArrowWorld : MonoBehaviour
{
    // Inspector: References
    [Header("References")]
    [Tooltip("Player transform. Arrow will rotate from the player position.")]
    public Transform player;

    [Tooltip("Manager that provides the closest collectible target.")]
    public TargetCollectibleManager targetManager;

    [Tooltip("Visual object to hide/show (child object with SpriteRenderer).")]
    public GameObject arrowVisual;

    // Inspector: Behaviour
    [Header("Behaviour")]
    [Tooltip("Offset from player where the arrow should appear.")]
    public Vector2 arrowOffset = new Vector2(0f, 0.8f);

    [Tooltip("Angle offset applied to arrow sprite (set if sprite points up/left by default).")]
    public float spriteAngleOffset = 0f;

    [Tooltip("Hide arrow visual if no target exists.")]
    public bool hideWhenNoTarget = true;

    private void Awake()
    {
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (targetManager == null)
            targetManager = TargetCollectibleManager.Instance;

        if (arrowVisual == null)
            arrowVisual = gameObject;
    }

    private void LateUpdate()
    {
        if (player == null) return;

        if (targetManager == null)
            targetManager = TargetCollectibleManager.Instance;

        Transform target = (targetManager != null)
            ? targetManager.GetCurrentTargetTransform(player.position)
            : null;

        // Always follow player position
        transform.position = player.position + new Vector3(arrowOffset.x, arrowOffset.y, 0f);

        // Hide / show based on toggle
        if (target == null)
        {
            if (arrowVisual != null)
                arrowVisual.SetActive(!hideWhenNoTarget);
            return;
        }

        if (arrowVisual != null && !arrowVisual.activeSelf)
            arrowVisual.SetActive(true);

        // Rotate toward target
        Vector2 dir = (Vector2)target.position - (Vector2)transform.position;
        if (dir.sqrMagnitude < 0.0001f) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + spriteAngleOffset);
    }
}
