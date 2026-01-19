# Unity Mobile Roguelike Prototype (Android)

A top-down mobile roguelike prototype built in **Unity (C#)** for Android.  
This project focuses on **core gameplay systems** rather than visual polish and is intended to demonstrate practical Unity mobile development, iteration, and debugging.

The project is currently a **work-in-progress prototype** and is not intended to represent a finished or monetised game.

---

## Overview

- **Platform:** Android  
- **Engine:** Unity  
- **Language:** C#  
- **Genre:** Top-down roguelike / arena survival  
- **Orientation:** Mobile (portrait)

Primary goals of this project:
- Build and iterate on real gameplay systems
- Work through broken implementations and refactors
- Maintain a playable state throughout development
- Demonstrate Unity mobile workflow and C# gameplay code

---

## Implemented Features

- Player movement adapted for mobile input
- Auto-targeting projectile weapon system
- Weapon cooldowns and lock-on timing
- Weapon rotation and firing constraints
- Enemy AI with chase behaviour
- Enemy and player health systems
- Contact-based damage handling
- World-space health bar UI
- Collectible system with nearest-target selection
- Target arrow indicator logic
- Tilemap-based forest environment
- Randomised ground generation
- Sorting layer configuration for 2D top-down rendering

---

## Tech & Tools

- Unity (C#)
- Android build pipeline
- Tilemap system
- SpriteRenderer and world-space UI
- Centralised gameplay managers
- Git version control

---

## Install & Play (Android APK)

1. Download the latest **APK** from the **GitHub Releases** section of this repository
2. On your Android device, enable **Install unknown apps** for your browser or file manager
3. Open the downloaded APK file and install
4. Launch the game from your app drawer

> This build is intended for testing and demonstration purposes.

---

## Run in Unity (Source)

1. Open the project in **Unity** (Android module installed)
2. Switch platform to **Android**
3. Open the main gameplay scene
4. Build & Run on an Android device

---

## Development Notes

This project was developed iteratively and includes:
- Multiple rewrites of gameplay systems
- Refactoring after incorrect architectural decisions
- Rebuilding UI and Tilemap setups after tooling issues
- Debugging collision, targeting, and rendering behaviour

These changes were part of the normal development process rather than following a single tutorial or predefined path.

---

## Planned Work (Next Iteration)

The following items represent planned or in-progress improvements.  
These are **not committed features** and may change as development continues.

- Add environmental props (trees / bushes)
- Implement Y-position based sorting for units and props
- Constrain enemy spawn locations based on environment
- Add a basic gameplay loop (waves / scaling difficulty)
- Perform mobile performance checks and optimisation
- Produce a final Android APK build

---

## Author

Min Soe  
Bachelor of Computer Science – University of Waikato
