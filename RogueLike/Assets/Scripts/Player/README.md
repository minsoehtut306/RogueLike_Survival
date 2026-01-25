# Player

This folder contains scripts related to the player character, including movement, health, and death handling.

## PlayerMovement.cs
Handles player movement and animation control:
- Reads input from a `DynamicJoystick` for mobile movement.
- Applies a deadzone and small delay to prevent animation flickering.
- Moves the player using `Rigidbody2D` for stable physics-based movement.
- Updates animator parameters (`MoveX`, `MoveY`, `Speed`) to control movement and idle animations.
- Automatically stops movement and animation updates when the player is dead.

## PlayerHealth.cs
Manages player health and death behaviour:
- Stores current and maximum health values.
- Applies damage with temporary invincibility frames.
- Triggers the player death animation using an Animator bool.
- Disables movement, weapons, physics motion, and colliders on death.
- Invokes an `onDeath` event to trigger Game Over UI or other systems.
