# Systems

This folder contains core gameplay systems that control spawning, game flow, camera behaviour, and pickups.

## GameManager.cs
Controls overall game progression:
- Manages difficulty scaling over time.
- Adjusts enemy spawn rates or limits as the game progresses.
- Acts as a central place for high-level game rules.

## EnemySpawner.cs
Handles enemy spawning logic:
- Spawns enemies around the player within a defined radius.
- Uses object pooling where possible to reduce runtime allocations.
- Works together with `GoblinPool` to reuse enemy instances efficiently.

## GoblinPool.cs
Provides a simple object pooling system for enemies:
- Pre-instantiates enemy objects.
- Reuses inactive enemies instead of destroying and re-instantiating.
- Improves performance, especially on mobile devices.

## FoxSpawner.cs
Controls collectible spawning:
- Spawns collectible objects (e.g., fox pickups) in the world.
- Ensures collectibles appear at valid positions.
- Acts as an entry point for granting new weapons to the player.

## TargetCollectible.cs
Represents a collectible pickup:
- Detects when the player reaches or collects the item.
- Notifies the manager system when collected.
- Usually triggers weapon unlocks or upgrades.

## TargetCollectibleManager.cs
Manages active collectibles:
- Keeps track of current collectible targets in the scene.
- Ensures only a valid number of collectibles exist at one time.
- Provides data for UI elements such as world arrows.

## CameraFollow2D.cs
Controls the camera behaviour:
- Smoothly follows the player position.
- Uses damping to avoid sudden camera snaps.
- Keeps gameplay centred on the player at all times.
