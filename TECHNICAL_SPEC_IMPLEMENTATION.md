# Technical Spec Implementation Guide

This document outlines how the game has been implemented according to the technical specification.

## Core Loop Implementation

✅ **Spawn zombies every 5s**
- `ZombieSpawner.cs` handles spawning with 5-second intervals
- First spawn delay: 5 seconds
- Spawns from `far_barrier` transform

✅ **Player aims with virtual joystick**
- `BowController.cs` uses `Joystick.cs` for aiming
- Crosshair positioned at screen center
- Joystick input converted to 3D aim direction

✅ **Player shoots arrow (cooldown: 5s)**
- `BowController.cs` implements 5-second cooldown
- Attack button triggers shooting
- Cooldown indicator shows progress

✅ **Zombie reaches front_barrier → apply damage (5 dmg/sec)**
- `ZombieController.cs` detects `FrontBarrier` collision
- Stops moving and plays attack animation
- Deals 5 damage per second to player

✅ **Every 10 kills → heal player (+10)**
- `GameManager.cs` tracks zombie kills
- Heals 10 HP every 10 kills
- Triggers healing event

✅ **Game over when player_health <= 0**
- `GameManager.cs` monitors health
- Triggers game over event
- Shows defeated screen

## Player System

### Health
- Max Health: 100 (configurable in `GameManager`)
- Healing: +10 HP every 10 kills

### Aiming
- Method: Virtual joystick (`Joystick.cs`)
- Crosshair: Screen center (UI element)
- Implementation: `BowController.cs` reads joystick direction

### Shooting
- Fire Button: Attack button (UI Button)
- Cooldown: 5 seconds (hardcoded in `BowController`)
- Arrow Damage: Random 10-30 (set per arrow in `BowController.ShootArrow()`)

## Zombie System

### Spawning
- Interval: 5 seconds (`ZombieSpawner.spawnInterval`)
- Spawn Point: `far_barrier` transform
- First Delay: 5 seconds (`ZombieSpawner.firstSpawnDelay`)

### Stats
- Health: Random 50-100 (`ZombieController` random on Awake)
- Move Speed: Constant (configurable)
- Score: 10 points per kill

### Damage to Player
- Damage: 5 per second (`ZombieController.damagePerSecond`)
- Only at barrier: Yes (checks `FrontBarrier` collision)

### Animations
- Idle: "idle" (Animator parameter)
- Move: "walk" (Animator parameter)
- Attack: "attack" (Animator parameter)
- Dead: "dead" (Animator parameter)

### VFX
- Death: "smoke_pop" effect (spawned on death)

## Barriers

### Front Barrier
- Script: `FrontBarrier.cs`
- Purpose: Prevents zombie entry
- On Enter: Zombie stops, plays attack, deals damage

### Wood Barrier
- Decoration + collision (no special script needed)

### Far Barrier
- Spawn point for zombies
- Assigned to `ZombieSpawner.farBarrier`

## Level: Abandoned Town

### Description
Narrow street level where player defends against zombies.

### Objects
- Zombies (spawned by `ZombieSpawner`)
- Front barrier (`FrontBarrier` component)
- Wood barriers (collision objects)
- Far barrier (spawn point)

### Camera
- Type: Fixed side view (3D side view)
- Set in scene camera settings

## UI Implementation

### Main Menu
- Buttons: Play, Settings, Quit
- BGM: `mainmenu.bgm` (looped)

### Settings
- Volume SFX: Slider (controls `AudioManager.sfxVolume`)
- Volume BGM: Slider (controls `AudioManager.bgmVolume`)

### Gameplay HUD
- Health Bar: `UIManager.healthBar` (Slider)
- Crosshair: `UIManager.crosshair` (RectTransform at screen center)
- Fire Button: Attack button (triggers `BowController.OnAttackButtonPressed()`)
- Cooldown Indicator: `UIManager.cooldownIndicator` (Image with fill)
- Score Counter: `UIManager.scoreText` (TextMeshPro)

### In-Game Settings
- Resume: `UIManager.ResumeGame()`
- Settings: Opens settings panel
- Quit: Returns to main menu

### Defeated Screen
- Score: `UIManager.finalScoreText`
- Restart Button: `UIManager.restartButton`
- BGM: `defeated.bgm` (not looped)

## Tutorial System

### Steps
1. Objective explanation
2. How to aim (virtual joystick)
3. How to shoot (attack button)
4. Healing mechanic info

### Implementation
- `TutorialManager.cs` handles tutorial flow
- Shows steps sequentially
- Skip button available
- Starts game after completion

## Audio System

### BGM
- `mainmenu.bgm`: Main menu (looped)
- `defeated.bgm`: Defeated screen (not looped)

### SFX
- `button_press.sfx`: All buttons
- `zombie_walk.sfx`: Zombie movement
- `zombie_dying.sfx`: Zombie death
- `arrow_shot.sfx`: Shooting
- `bow_loading.sfx`: Bow cooldown (optional)

### Implementation
- `AudioManager.cs` manages all audio
- Singleton pattern
- Dictionary-based SFX lookup
- Volume controls for BGM and SFX

## Resolution & Platform

- Resolution: 1280×720 (set in Player Settings)
- Platform: iOS
- Perspective: 3D side view

## Scene Setup Checklist

- [ ] Create GameManager GameObject with `GameManager` script
- [ ] Create Player GameObject with `BowController` script
- [ ] Create Bow model and assign to `BowController.bowTransform`
- [ ] Create ArrowSpawnPoint (empty GameObject)
- [ ] Create ZombieSpawner with `ZombieSpawner` script
- [ ] Create FarBarrier (spawn point) and assign to `ZombieSpawner.farBarrier`
- [ ] Create FrontBarrier with `FrontBarrier` script and Collider (trigger)
- [ ] Create Wood barriers (collision objects)
- [ ] Setup NavMesh for zombie pathfinding
- [ ] Create UI Canvas with `UIManager` script
- [ ] Add Health Bar (Slider)
- [ ] Add Crosshair (Image/RectTransform at screen center)
- [ ] Add Attack Button (Button)
- [ ] Add Cooldown Indicator (Image with Image Type = Filled)
- [ ] Add Score Text (TextMeshPro)
- [ ] Add Survival Time Text (TextMeshPro)
- [ ] Add Game Over Panel with restart button
- [ ] Add Pause Panel with resume button
- [ ] Create Joystick UI with `Joystick` script
- [ ] Assign joystick to `BowController.aimJoystick`
- [ ] Create Tutorial Panel with `TutorialManager` script
- [ ] Create AudioManager GameObject with `AudioManager` script
- [ ] Assign audio clips to `AudioManager`
- [ ] Set camera to fixed side view (3D)
- [ ] Set resolution to 1280×720 in Player Settings

## Prefabs to Create

1. **Arrow Prefab**
   - 3D arrow model
   - `Arrow` script
   - Rigidbody
   - Collider (trigger)

2. **Zombie Prefab**
   - 3D zombie model
   - `ZombieController` script
   - NavMeshAgent
   - Collider
   - Animator with animations: idle, walk, attack, dead

3. **Death VFX Prefab**
   - "smoke_pop" particle effect
   - Assign to `ZombieController.deathVFX`

## Notes

- All scripts use namespaces: `HordeInTown.Player`, `HordeInTown.Zombies`, etc.
- Singleton pattern used for `GameManager` and `AudioManager`
- Event system used for UI updates
- Mobile-first design with iOS optimization

