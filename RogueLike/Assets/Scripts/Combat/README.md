# Combat

This folder contains weapon and attack-related scripts.

## ArrowWeapon.cs
Controls an orbiting arrow weapon:
- Scans for the nearest enemy within a radius.
- Locks onto a target briefly (aiming delay).
- Fires as a projectile, applies damage on hit, then snaps back into orbit.
- Uses a trigger collider only while shooting to avoid accidental hits while orbiting.

## WeaponManager.cs
Manages the player’s weapons:
- Spawns weapon prefabs (e.g., fire arrow / water arrow) when the player collects a pickup.
- Keeps all weapons parented to the player.
- Positions weapons around the player in an arc/ellipse layout so they spread out neatly.
