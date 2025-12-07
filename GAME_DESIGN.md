# Horde In Town - Game Design Document

## Game Overview

**Genre:** 3D Tower Defense / Survival  
**Platform:** iOS (Primary)  
**Engine:** Unity 6000.3.0f1

## Core Concept

Horde In Town is a 3D tower defense survival game where the player controls a bow to defend against waves of zombies. The goal is to survive as long as possible by eliminating zombies before they reach the player's area.

## Core Mechanics

### Player Controls
- **Bow Control:** Player aims and shoots arrows using the bow
- **Movement:** Player may need to position themselves strategically (TBD)
- **Aiming:** Touch/joystick controls for aiming the bow
- **Shooting:** Tap/button to release arrows

### Gameplay Loop
1. Zombies spawn and approach the player's area
2. Player aims and shoots arrows to kill zombies
3. Survive as long as possible
4. Score/points based on zombies killed and survival time
5. Difficulty increases over time (more zombies, faster spawns)

### Zombie System
- Zombies spawn from various points around the map
- Move toward the player/defense area
- Different zombie types possible (TBD)
- Health system - zombies take damage from arrows

### Survival Mechanics
- Health/HP system for player
- If zombies reach player area, player takes damage
- Game over when player health reaches zero
- Score tracking based on:
  - Zombies killed
  - Survival time
  - Accuracy/headshots (TBD)

## Game Systems

### Core Systems
1. **Bow/Archery System**
   - Aiming mechanics
   - Arrow physics and trajectory
   - Draw strength/power system
   - Reload/cooldown timing

2. **Zombie Spawning System**
   - Wave-based or continuous spawning
   - Spawn points around the map
   - Spawn rate increases over time

3. **Combat System**
   - Arrow collision detection
   - Damage calculation
   - Zombie death animations/effects

4. **Game Manager**
   - Game state management (Menu, Playing, Paused, Game Over)
   - Score tracking
   - Wave/timer management
   - Difficulty scaling

5. **UI System**
   - Health bar
   - Score display
   - Ammo/arrow count (if applicable)
   - Game over screen
   - Pause menu

## Technical Requirements

### Controls (iOS)
- Touch-based aiming (joystick or direct touch)
- Tap to shoot
- UI buttons for pause, settings, etc.

### Performance
- Optimized for mobile devices
- Efficient zombie spawning/despawning
- Object pooling for arrows and zombies
- LOD system for 3D models

### Assets Needed
- 3D bow model and animations
- 3D zombie models and animations
- Arrow prefab
- Environment/arena models
- UI elements (already in RawAssets)
- Sound effects (already in RawAssets)
- Background music (already in RawAssets)

## Future Enhancements (Optional)
- Different arrow types
- Power-ups
- Upgrade system
- Multiple arenas/maps
- Leaderboards
- Achievements

