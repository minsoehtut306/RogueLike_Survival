# Enemies

This folder contains scripts that control enemy behaviour, health, and interactions with the player.

## GoblinChase.cs
Handles enemy movement and chasing logic:
- Moves the enemy toward the player using Rigidbody2D.
- Stops when close enough to avoid jitter.
- Updates animator parameters for movement direction and facing.
- Pauses movement automatically while the enemy is in a hurt state.

## EnemyLife.cs
Manages enemy health and life cycle:
- Stores and updates enemy health.
- Handles damage, hurt lock (temporary stun), and death.
- Disables movement and contact damage when the enemy dies.
- Triggers death animation and destroys the enemy via animation event or fallback timer.

## ContactDamage.cs
Applies damage to the player on contact:
- Deals periodic damage while the player stays in collision or trigger range.
- Uses a cooldown timer to prevent damage every frame.
- Supports player colliders on child objects by resolving PlayerHealth from parents.
