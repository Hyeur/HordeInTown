# Scene Setup Guide - Step by Step

This guide will walk you through setting up the main gameplay scene for Horde In Town.

## Step 1: Create New Scene

1. In Unity, go to **File > New Scene**
2. Choose **3D (URP)** or **3D Core** template
3. Save as `Assets/Scenes/Gameplay.unity`

## Step 2: Set Up Camera (Fixed Side View)

1. Select **Main Camera** in Hierarchy
2. Position: `(0, 5, -10)` (adjust for side view)
3. Rotation: `(15, 0, 0)` (looking down at scene)
4. Set **Projection** to **Perspective**
5. **Field of View**: 60 (adjust as needed)
6. This creates the 3D side view perspective

## Step 3: Create Ground/Environment

1. **Create Ground Plane:**
   - Right-click Hierarchy > **3D Object > Plane**
   - Rename to `Ground`
   - Scale: `(10, 1, 10)` (or adjust to your level size)
   - Position: `(0, 0, 0)`
   - Tag: `Ground`

2. **Add Collider:**
   - Ground should have a **Mesh Collider** (auto-added)
   - This is for arrow collision

3. **Set Up NavMesh:**
   - Select **Ground**
   - In Inspector, check **Navigation Static**
   - Go to **Window > AI > Navigation**
   - Click **Bake** tab
   - Click **Bake** button
   - This allows zombies to pathfind

## Step 4: Create Barriers

### Front Barrier (Defense Line)

1. **Create Front Barrier:**
   - Right-click Hierarchy > **3D Object > Cube**
   - Rename to `FrontBarrier`
   - Position: `(0, 1, -5)` (in front of player, adjust Z as needed)
   - Scale: `(20, 2, 0.5)` (wide barrier)
   - Tag: `FrontBarrier`

2. **Add Components:**
   - Add **Box Collider** (if not present)
   - Check **Is Trigger** ✓
   - Add **FrontBarrier** script (`HordeInTown.Combat.FrontBarrier`)

### Wood Barriers (Decoration)

1. **Create Wood Barriers:**
   - Right-click Hierarchy > **3D Object > Cube**
   - Rename to `WoodBarrier1`
   - Position: `(-8, 1, -3)` (left side)
   - Scale: `(2, 2, 1)`
   - Add **Box Collider** (not trigger)
   - Duplicate and position on right side: `(8, 1, -3)`

### Far Barrier (Spawn Point)

1. **Create Far Barrier:**
   - Right-click Hierarchy > **3D Object > Cube** (or Empty GameObject)
   - Rename to `FarBarrier`
   - Position: `(0, 0, 15)` (far from player, adjust Z as needed)
   - This is where zombies spawn
   - You can make it invisible or decorative

## Step 5: Create Player Setup

1. **Create Player GameObject:**
   - Right-click Hierarchy > **Create Empty**
   - Rename to `Player`
   - Position: `(0, 0, 0)` (center of scene)
   - Tag: `Player`

2. **Add BowController Script:**
   - Select **Player**
   - Add Component > **Bow Controller** (`HordeInTown.Player.BowController`)

3. **Create Bow Model:**
   - Right-click **Player** > **Create Empty**
   - Rename to `Bow`
   - Position: `(0, 1.5, 0)` (in front of player)
   - This will hold your 3D bow model
   - Assign to `BowController.bowTransform`

4. **Create Arrow Spawn Point:**
   - Right-click **Bow** > **Create Empty**
   - Rename to `ArrowSpawnPoint`
   - Position: `(0, 0, 0.5)` (in front of bow)
   - Assign to `BowController.arrowSpawnPoint`

5. **Assign Camera:**
   - Drag **Main Camera** to `BowController.playerCamera`

6. **Configure BowController:**
   - **Shoot Cooldown**: 5
   - **Min Arrow Damage**: 10
   - **Max Arrow Damage**: 30
   - **Use Joystick**: ✓ (checked)
   - **Aim Joystick**: (assign from UI later)
   - **Attack Button**: (assign from UI later)
   - **Crosshair**: (assign from UI later)
   - **Cooldown Indicator**: (assign from UI later)

## Step 6: Create Zombie Spawner

1. **Create ZombieSpawner:**
   - Right-click Hierarchy > **Create Empty**
   - Rename to `ZombieSpawner`
   - Position: `(0, 0, 0)`

2. **Add ZombieSpawner Script:**
   - Add Component > **Zombie Spawner** (`HordeInTown.Zombies.ZombieSpawner`)

3. **Configure ZombieSpawner:**
   - **Zombie Prefab**: (assign zombie prefab later)
   - **Far Barrier**: Drag `FarBarrier` transform here
   - **Spawn Interval**: 5
   - **First Spawn Delay**: 5

## Step 7: Create GameManager

1. **Create GameManager:**
   - Right-click Hierarchy > **Create Empty**
   - Rename to `GameManager`
   - Position: `(0, 0, 0)`

2. **Add GameManager Script:**
   - Add Component > **Game Manager** (`HordeInTown.Managers.GameManager`)

3. **Configure GameManager:**
   - **Max Player Health**: 100
   - **Heal Per 10 Kills**: 10

## Step 8: Create AudioManager

1. **Create AudioManager:**
   - Right-click Hierarchy > **Create Empty**
   - Rename to `AudioManager`
   - Position: `(0, 0, 0)`

2. **Add AudioManager Script:**
   - Add Component > **Audio Manager** (`HordeInTown.Managers.AudioManager`)

3. **Add Audio Sources:**
   - The script will auto-create them, or manually:
   - Add Component > **Audio Source** (rename to BGM Source)
   - Add Component > **Audio Source** (rename to SFX Source)
   - Assign to `AudioManager.bgmSource` and `sfxSource`

4. **Import Audio Files:**
   - Go to `RawAssets/Sound/`
   - Import audio files to Unity
   - Assign to AudioManager:
     - **Main Menu BGM**: `mainmenu.bgm`
     - **Defeated BGM**: `defeated.bgm`
     - **Button Press SFX**: `button_press.sfx`
     - **Zombie Walk SFX**: `zombie_walk.sfx`
     - **Zombie Death SFX**: `zombie_dying.sfx`
     - **Arrow Shot SFX**: `arrow_shot.sfx`
     - **Bow Loading SFX**: `bow_loading.sfx`

## Step 9: Create Prefabs

### Arrow Prefab

1. **Create Arrow:**
   - Right-click Hierarchy > **3D Object > Cylinder**
   - Rename to `Arrow`
   - Scale: `(0.1, 0.5, 0.1)` (thin and long)
   - Rotation: `(90, 0, 0)` (pointing forward)

2. **Add Components:**
   - Add **Rigidbody**
     - Mass: 0.1
     - Drag: 0.5
     - Use Gravity: ✓
   - Add **Capsule Collider**
     - Check **Is Trigger** ✓
   - Add **Arrow** script (`HordeInTown.Combat.Arrow`)
     - **Damage**: 20 (will be overridden by BowController)
     - **Lifetime**: 10

3. **Create Prefab:**
   - Drag `Arrow` from Hierarchy to `Assets/Prefabs/`
   - Delete from scene (we'll instantiate it)

### Zombie Prefab

1. **Create Zombie:**
   - Import your 3D zombie model OR
   - Use placeholder: Right-click Hierarchy > **3D Object > Capsule**
   - Rename to `Zombie`
   - Position: `(0, 1, 0)` (so feet are on ground)

2. **Add Components:**
   - Add **NavMeshAgent**
     - Radius: 0.5
     - Height: 2
     - Speed: 2
   - Add **Capsule Collider**
     - Check **Is Trigger** ✓
   - Add **Animator** (if you have animations)
   - Add **ZombieController** script (`HordeInTown.Zombies.ZombieController`)
     - **Min Health**: 50
     - **Max Health**: 100
     - **Move Speed**: 2
     - **Score Value**: 10
     - **Damage Per Second**: 5
     - **Damage Interval**: 1
     - **Death VFX**: (assign smoke_pop prefab later)

3. **Create Prefab:**
   - Drag `Zombie` from Hierarchy to `Assets/Prefabs/`
   - Delete from scene

4. **Assign to ZombieSpawner:**
   - Select `ZombieSpawner`
   - Drag `Zombie` prefab to `Zombie Prefab` field

## Step 10: Connect UI to Scripts

1. **Select Canvas** in Hierarchy

2. **Find UIManager:**
   - If not added, add **UI Manager** script to Canvas

3. **Assign UI References:**
   - **Health Bar**: Drag health bar Slider
   - **Health Bar Fill**: Drag fill Image
   - **Score Text**: Drag score TextMeshPro
   - **Survival Time Text**: Drag time TextMeshPro
   - **Crosshair**: Drag crosshair RectTransform
   - **Cooldown Indicator**: Drag cooldown Image
   - **Game Over Panel**: Drag game over panel
   - **Final Score Text**: Drag final score text
   - **Final Time Text**: Drag final time text
   - **Restart Button**: Drag restart button
   - **Pause Button**: Drag pause button
   - **Resume Button**: Drag resume button

4. **Connect BowController to UI:**
   - Select `Player`
   - **Aim Joystick**: Drag joystick GameObject (with Joystick script)
   - **Attack Button**: Drag attack button
   - **Crosshair**: Drag crosshair RectTransform
   - **Cooldown Indicator**: Drag cooldown indicator Image

## Step 11: Set Up Joystick

1. **Create Joystick UI:**
   - Right-click Canvas > **UI > Image** (for background)
   - Rename to `JoystickBackground`
   - Position: Bottom-left of screen
   - Size: `(150, 150)`
   - Add **Joystick** script (`HordeInTown.UI.Joystick`)
   
2. **Create Joystick Handle:**
   - Right-click `JoystickBackground` > **UI > Image** (for handle)
   - Rename to `JoystickHandle`
   - Size: `(50, 50)`
   - Position: Center of background

3. **Configure Joystick Script:**
   - **Joystick Background**: Drag `JoystickBackground`
   - **Joystick Handle**: Drag `JoystickHandle`
   - **Joystick Range**: 50

## Step 12: Set Up Tutorial (Optional)

1. **Create Tutorial Panel:**
   - Right-click Canvas > **UI > Panel**
   - Rename to `TutorialPanel`
   - Add **Tutorial Manager** script (`HordeInTown.UI.TutorialManager`)

2. **Add Tutorial Elements:**
   - Add TextMeshPro for tutorial text
   - Add "Next" button
   - Add "Skip" button
   - Assign to TutorialManager fields

## Step 13: Configure Project Settings

1. **Set Resolution:**
   - **Edit > Project Settings > Player**
   - **Resolution and Presentation**
   - **Default Canvas Scaler**: Set to 1280×720
   - Or set in **Build Settings** when building

2. **Set Tags:**
   - **Edit > Project Settings > Tags and Layers**
   - Add tags: `Player`, `Zombie`, `Ground`, `Environment`, `FrontBarrier`

3. **Set Layers (Optional):**
   - Add layers: `Zombie`, `Ground`, `Player`

## Step 14: Test Scene

1. **Press Play**
2. **Check:**
   - Camera shows side view
   - Joystick appears and works
   - Attack button works
   - Zombies spawn after 5 seconds
   - Zombies move toward player
   - Arrows can be shot
   - Zombies take damage from arrows
   - Health bar updates
   - Score updates

## Step 15: Polish

1. **Add Lighting:**
   - Add **Directional Light**
   - Position: `(0, 10, 0)`
   - Rotation: `(50, -30, 0)`

2. **Add Skybox:**
   - **Window > Rendering > Lighting**
   - Assign skybox material

3. **Add Particle Effects:**
   - Create death VFX (smoke_pop)
   - Assign to `ZombieController.deathVFX`

4. **Import 3D Models:**
   - Replace placeholder bow with actual model
   - Replace placeholder zombie with actual model
   - Add environment props

## Quick Checklist

- [ ] Scene created and saved
- [ ] Camera set to side view
- [ ] Ground created with NavMesh baked
- [ ] Front barrier created with FrontBarrier script
- [ ] Far barrier created (spawn point)
- [ ] Player GameObject with BowController
- [ ] Bow model and ArrowSpawnPoint created
- [ ] ZombieSpawner created and configured
- [ ] GameManager created
- [ ] AudioManager created with audio clips
- [ ] Arrow prefab created
- [ ] Zombie prefab created
- [ ] UI connected to scripts
- [ ] Joystick set up
- [ ] Tags and layers configured
- [ ] Scene tested and working

## Troubleshooting

**Zombies don't spawn:**
- Check ZombieSpawner has zombie prefab assigned
- Check FarBarrier is assigned
- Check NavMesh is baked

**Arrows don't shoot:**
- Check Arrow prefab is assigned to BowController
- Check ArrowSpawnPoint is assigned
- Check attack button is connected

**Zombies don't move:**
- Check NavMesh is baked
- Check zombie has NavMeshAgent
- Check target is assigned

**UI doesn't update:**
- Check UIManager is on Canvas
- Check all UI references are assigned
- Check GameManager events are firing

