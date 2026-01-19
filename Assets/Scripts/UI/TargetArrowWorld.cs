using UnityEngine;
using UnityEngine.UI;

public class TargetArrowWorld : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public TargetCollectibleManager manager;

    [Header("Position")]
    public Vector3 offset = new Vector3(0f, 0.8f, 0f);

    [Header("Behaviour")]
    public bool hideIfNoTarget = true;
    public float rotationOffsetDeg = 0f;
    public float graceSeconds = 0.25f;

    Transform currentTarget;
    float lastSeenTargetTime = -999f;

    SpriteRenderer sr;
    Image img;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        img = GetComponent<Image>();
    }

    void Start()
    {
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (manager == null)
            manager = TargetCollectibleManager.Instance;
    }

    void LateUpdate()
    {
        if (player == null)
        {
            SetVisible(false);
            return;
        }

        // Follow player
        transform.position = player.position + offset;

        // Get target
        Transform t = null;
        if (manager != null)
            t = manager.GetCurrentTargetTransform(player.position);

        if (t != null)
        {
            currentTarget = t;
            lastSeenTargetTime = Time.time;
        }

        bool hasTargetRecently = (currentTarget != null) && (Time.time - lastSeenTargetTime <= graceSeconds);
        if (hideIfNoTarget && !hasTargetRecently)
        {
            SetVisible(false);
            return;
        }

        SetVisible(true);

        // Rotate toward target if exists
        if (currentTarget != null)
        {
            Vector2 dir = (currentTarget.position - player.position);
            if (dir.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffsetDeg);
            }
        }
    }

    void SetVisible(bool on)
    {
        if (sr != null) sr.enabled = on;
        if (img != null) img.enabled = on;
    }
}
