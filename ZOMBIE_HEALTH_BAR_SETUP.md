# Zombie Health Bar Setup Guide

## Overview

The zombie health bar system displays a health bar above each zombie's head that updates in real-time as they take damage.

## Files Created

- `Assets/Scripts/UI/ZombieHealthBar.cs` - Health bar controller script
- Updated `Assets/Scripts/Zombies/ZombieController.cs` - Integrated health bar updates

## Setup Instructions

### Step 1: Create Health Bar Prefab

1. **Create Health Bar GameObject:**
   - Right-click in Hierarchy > **UI > Canvas**
   - Rename to `ZombieHealthBar`
   - Set Canvas **Render Mode** to **World Space**
   - Set **Event Camera** to Main Camera

2. **Configure Canvas:**
   - **Rect Transform:**
     - Width: `1`
     - Height: `0.2`
     - Scale: `(0.01, 0.01, 0.01)` (adjust for size)

3. **Add Health Bar UI:**
   - Right-click `ZombieHealthBar` > **UI > Slider**
   - Rename to `HealthBar`
   - Configure Slider:
     - **Min Value**: 0
     - **Max Value**: 100
     - **Value**: 100
     - Remove Handle (not needed)
     - Set **Fill Area** color (green/red gradient)

4. **Add Fill Image:**
   - Select **Fill** child of Slider
   - Set color to green (or use gradient)

5. **Optional - Add Health Text:**
   - Right-click `ZombieHealthBar` > **UI > Text - TextMeshPro**
   - Position above or on health bar
   - Set font size and color

6. **Add ZombieHealthBar Script:**
   - Select `ZombieHealthBar` Canvas
   - Add Component > **Zombie Health Bar** (`HordeInTown.UI.ZombieHealthBar`)

7. **Configure ZombieHealthBar Script:**
   - **Health Bar**: Drag the Slider
   - **Health Bar Fill**: Drag the Fill Image
   - **Health Bar Canvas**: Drag the Canvas (or leave empty, will auto-find)
   - **Offset Y**: `2.5` (height above zombie head)
   - **Always Face Camera**: ✓ (checked)
   - **Healthy Color**: Green
   - **Low Health Color**: Red
   - **Health Text**: (optional) Drag TextMeshPro if you added it

8. **Create Prefab:**
   - Drag `ZombieHealthBar` from Hierarchy to `Assets/Prefabs/`
   - Delete from scene

### Step 2: Add to Zombie Prefab

1. **Open Zombie Prefab** (or select zombie in scene)

2. **Add Health Bar:**
   - Right-click Zombie root > **Create Empty**
   - Rename to `HealthBar`
   - Drag `ZombieHealthBar` prefab as child of `HealthBar`
   - Position: `(0, 2.5, 0)` (above head)

3. **Configure ZombieController:**
   - Select Zombie root
   - In **ZombieController** component:
     - **Health Bar**: Drag the `HealthBar` GameObject (with ZombieHealthBar script)

### Step 3: Alternative Setup (Simpler)

If you prefer a simpler setup without a prefab:

1. **On Zombie GameObject:**
   - Add Component > **Zombie Health Bar**
   - Create UI Canvas as child (World Space)
   - Add Slider to canvas
   - Assign references in ZombieHealthBar component

## Features

- **Automatic Positioning**: Health bar positions above zombie's head
- **Head Bone Detection**: If zombie has humanoid rig, uses head bone position
- **Camera Facing**: Health bar always faces camera (billboard effect)
- **Color Gradient**: Changes from green (healthy) to red (low health)
- **Real-time Updates**: Updates automatically when zombie takes damage
- **Auto-hide on Death**: Hides when zombie dies

## Configuration Options

### ZombieHealthBar Settings:
- **Offset Y**: Height above zombie (default: 2.5)
- **Always Face Camera**: Makes health bar always face player
- **Healthy Color**: Color when health is full
- **Low Health Color**: Color when health is low
- **Health Text**: Optional text showing "100/100" format

### ZombieController Integration:
- Health bar automatically updates when `TakeDamage()` is called
- Health bar hides when zombie dies
- Max health is set automatically based on zombie's random health

## Troubleshooting

**Health bar doesn't appear:**
- Check Canvas Render Mode is "World Space"
- Check Canvas Scale is appropriate (0.01, 0.01, 0.01)
- Verify ZombieHealthBar script is assigned
- Check Offset Y value

**Health bar too big/small:**
- Adjust Canvas Scale
- Or adjust individual UI element sizes

**Health bar not updating:**
- Verify ZombieController has healthBar assigned
- Check that TakeDamage() is being called
- Verify health bar references are assigned

**Health bar in wrong position:**
- Adjust Offset Y
- Check if head bone detection is working (for humanoid rigs)
- Manually position if needed

## Notes

- Health bar uses World Space Canvas for 3D positioning
- Works with both humanoid and generic rigs
- Automatically finds camera if not assigned
- Health bar is destroyed with zombie after death delay

