# UI

This folder contains scripts used for user interface elements (HUD, joystick input, targeting indicators, and game-over screen).

## HealthBarUI.cs
Updates the health bar UI:
- Reads HP values from `PlayerHealth`.
- Converts HP to a 0–1 value and updates `Image.fillAmount`.
- Designed for Unity UI images set to `Filled` type.

## DynamicJoystick.cs
Implements a touch joystick for mobile input:
- Appears where the player touches the screen (dynamic placement).
- Knob movement is clamped within a radius.
- Outputs a normalised input vector (`Input`) used by `PlayerMovement`.

## TargetArrowWorld.cs
Displays a world arrow that points to the nearest collectible target:
- Queries `TargetCollectibleManager` to find the closest active collectible.
- Rotates the arrow toward the target.
- Can hide the arrow when no target exists.

## GameOverUI.cs
Shows the Game Over screen and handles buttons:
- Displays the Game Over panel when called (usually via `PlayerHealth.onDeath`).
- Optional pause using `Time.timeScale`.
- Includes button callbacks for restarting or quitting.
