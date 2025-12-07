# Scene Setup Guide

This guide will help you set up the main gameplay scene for Horde In Town.

## Scene Structure

### Required GameObjects

1. **GameManager** (Empty GameObject)
   - Add `GameManager` script
   - Tag: "GameManager"
   - This will be a singleton that manages game state

2. **Player** (Empty GameObject or with model)
   - Position: Center of the arena (0, 0, 0)
   - Tag: "Player"
   - Add `BowController` script
   - Child objects:
     - **Bow** (3D model)
       - Assign to BowController's `bowTransform`
     - **ArrowSpawnPoint** (Empty GameObject)
       - Position: Where arrows should spawn from the bow
       - Assign to BowController's `arrowSpawnPoint`

3. **Main Camera**
   - Position: Behind/above player
   - Set as player camera in BowController
   - Configure for 3D view

4. **ZombieSpawner** (Empty GameObject)
   - Add `ZombieSpawner` script
   - Create child empty GameObjects for spawn points:
     - **SpawnPoint1**, **SpawnPoint2**, etc.
     - Position them around the map perimeter
     - Assign to ZombieSpawner's `spawnPoints` array

5. **UI Canvas**
   - Create Canvas (UI > Canvas)
   - Add `UIManager` script to Canvas or child GameObject
   - UI Elements needed:
     - **HealthBar** (Slider)
     - **ScoreText** (TextMeshPro)
     - **SurvivalTimeText** (TextMeshPro)
     - **GameOverPanel** (Panel with restart/menu buttons)
     - **PausePanel** (Panel with resume button)
     - **PauseButton** (Button)

6. **NavMesh Setup**
   - Select ground/floor GameObject
   - Window > AI > Navigation
   - Mark as Navigation Static
   - Bake NavMesh
   - This allows zombies to pathfind to the player

7. **Ground/Environment**
   - Create or import ground plane
   - Tag: "Ground" or "Environment"
   - Add colliders for arrow collision
   - Mark as Navigation Static for NavMesh

## Prefabs to Create

1. **Arrow Prefab**
   - Create 3D arrow model or use primitive (Cylinder)
   - Add `Arrow` script
   - Add Rigidbody component
   - Add Collider (Capsule or Box)
   - Set as Trigger
   - Assign to BowController's `arrowPrefab`

2. **Zombie Prefab**
   - Create or import 3D zombie model
   - Add `ZombieController` script
   - Add NavMeshAgent component
   - Add Collider (Capsule)
   - Tag: "Zombie"
   - Assign to ZombieSpawner's `zombiePrefab`

3. **HealthBar Prefab** (for zombies)
   - Create UI Canvas or use world space UI
   - Add Slider component
   - Assign to ZombieController's `healthBarPrefab`

## Layer Setup

Create these layers in Edit > Project Settings > Tags and Layers:
- **Zombie** (Layer 8)
- **Ground** (Layer 9)
- **Player** (Layer 10)

Set up Layer Masks:
- In BowController, set `aimLayerMask` to include Ground layer
- In Arrow, set `zombieLayer` to Zombie layer

## Tags Setup

Create these tags:
- **Player**
- **Zombie**
- **Ground**
- **Environment**
- **DefensePoint** (if using a defense point instead of player)

## Mobile Controls Setup

For iOS/mobile controls:

1. **Joystick Setup**
   - Import a joystick asset (or create one)
   - Add Joystick component to UI
   - Assign to BowController's `aimJoystick`
   - Set `useJoystick = true` in BowController

2. **Touch Controls**
   - The BowController already handles touch input
   - Tap screen to shoot
   - Joystick for aiming

## Testing Checklist

- [ ] Player can aim bow
- [ ] Arrows spawn and fly correctly
- [ ] Zombies spawn at spawn points
- [ ] Zombies move toward player
- [ ] Arrows damage/kill zombies
- [ ] Score increases when zombies die
- [ ] Health decreases when zombies reach player
- [ ] Game over screen appears when health reaches 0
- [ ] UI updates correctly (health, score, time)
- [ ] Pause functionality works
- [ ] NavMesh is baked correctly

## Next Steps

1. Import 3D models (bow, zombie, arrow, environment)
2. Set up animations for bow drawing and zombie walking
3. Add sound effects (already in RawAssets)
4. Polish UI design (use assets from RawAssets/2D/UI)
5. Add particle effects for arrow hits
6. Implement object pooling for better performance

