using UnityEngine;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Prefabs")]
    public GameObject fireArrowPrefab;
    public GameObject waterArrowPrefab;

    [Header("Limits")]
    public int maxWeapons = 8;

    [Header("Orbit Layout")]
    public float weaponRadius = 0.75f;
    public Vector2 orbitCenterOffset = new Vector2(0f, -0.15f);
    public float yEllipseScale = 1.0f;

    [Header("Orbit Arc (no weapons below player)")]
    public float arcStartDeg = 30f;   // right-up
    public float arcEndDeg = 330f;    // left-up

    private readonly List<GameObject> weapons = new();
    private int pickupIndex = 0;

    // =======================
    // Public API
    // =======================
    public void GiveNextWeapon()
    {
        if (weapons.Count >= maxWeapons) return;

        GameObject prefab = (pickupIndex % 2 == 0)
            ? fireArrowPrefab
            : waterArrowPrefab;

        pickupIndex++;

        GameObject weapon = Instantiate(prefab, transform);
        weapons.Add(weapon);

        var aw = weapon.GetComponent<ArrowWeapon>();
        if (aw != null) aw.player = transform;

        RepositionWeapons();
    }

    // =======================
    // Positioning
    // =======================
    private void RepositionWeapons()
    {
        int count = weapons.Count;
        if (count == 0) return;

        float arcRange = arcEndDeg - arcStartDeg;
        if (arcRange <= 0f) arcRange += 360f;

        // First weapon goes to the middle of the arc (LEFT side)
        float step = (count == 1) ? 0f : arcRange / (count - 1);

        for (int i = 0; i < count; i++)
        {
            float angleDeg = (count == 1)
                ? arcStartDeg + arcRange * 0.5f
                : arcStartDeg + step * i;

            float rad = angleDeg * Mathf.Deg2Rad;

            float x = Mathf.Cos(rad) * weaponRadius;
            float y = Mathf.Sin(rad) * weaponRadius * yEllipseScale;

            Vector3 localPos = new Vector3(
                orbitCenterOffset.x + x,
                orbitCenterOffset.y + y,
                0f
            );

            weapons[i].transform.localPosition = localPos;
        }
    }
}
