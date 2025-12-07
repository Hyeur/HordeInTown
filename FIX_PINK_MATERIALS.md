# Fix Pink Materials in Unity

Pink materials indicate a missing or incompatible shader. Here's how to fix it:

## Quick Fix

### Option 1: Change Shader to Standard (Built-in Render Pipeline)

1. **Select the pink material(s):**
   - In Project window, find the material (usually in Materials folder of imported asset)
   - Or select the GameObject with pink material in Hierarchy

2. **Change Shader:**
   - In Inspector, find the **Shader** dropdown
   - Change from current shader to: **Standard** (or **Standard (Specular setup)**)
   - The material should now appear correctly

### Option 2: Use URP Lit Shader (Universal Render Pipeline)

If your project uses URP (Universal Render Pipeline):

1. **Select the material**
2. **Change Shader to:**
   - **Universal Render Pipeline > Lit**
   - Or **URP > Lit** (depending on Unity version)

### Option 3: Reimport Material Textures

Sometimes textures aren't imported correctly:

1. **Select the material**
2. **Check texture slots:**
   - Albedo/Diffuse
   - Normal Map
   - Metallic/Smoothness
3. **If textures are missing:**
   - Find texture files in the imported asset folder
   - Drag them to the appropriate slots
   - Or click the circle icon next to texture slot and select texture

## Step-by-Step Fix

### Method 1: Fix Individual Materials

1. **Locate Materials:**
   - In Project window, go to the imported asset folder
   - Look for `.mat` files (materials)
   - Or check the imported model's Materials folder

2. **Select Each Material:**
   - Click on a material file
   - In Inspector, you'll see it's pink

3. **Change Shader:**
   - At the top of Inspector, find **Shader** dropdown
   - Click and select: **Standard** (for Built-in RP)
   - Or: **Universal Render Pipeline > Lit** (for URP)

4. **Assign Textures:**
   - If textures are available, drag them to:
     - **Albedo** (main color texture)
     - **Normal Map** (if available)
     - **Metallic** (if available)

### Method 2: Fix All Materials at Once

1. **Select the imported model folder** in Project window
2. **In Inspector, check Import Settings:**
   - Look for **Materials** section
   - Change **Material Creation Mode** to:
     - **Standard** (Built-in RP)
     - Or **URP** (if using Universal Render Pipeline)
3. **Click Apply**
4. **Reimport the model** if needed

### Method 3: Create New Material

If the material is completely broken:

1. **Create New Material:**
   - Right-click in Project > **Create > Material**
   - Name it (e.g., "ZombieMaterial")

2. **Configure Material:**
   - Shader: **Standard** or **URP > Lit**
   - Assign textures if available
   - Adjust color if needed

3. **Assign to Model:**
   - Select the GameObject with pink material
   - In Inspector, find **Mesh Renderer** component
   - Drag new material to **Materials** slot

## Check Your Render Pipeline

Your project might be using URP (Universal Render Pipeline). Check:

1. **Edit > Project Settings > Graphics**
2. **Scriptable Render Pipeline Settings:**
   - If it shows "UniversalRenderPipelineAsset", you're using URP
   - Use **URP > Lit** shader for materials
   - If empty, you're using Built-in RP
   - Use **Standard** shader for materials

## For Your Game (iOS - Unity 6000.3.0f1)

Since you're using Unity 6000.3.0f1, you're likely using URP:

1. **Select pink material**
2. **Change Shader to: Universal Render Pipeline > Lit**
3. **Assign textures** if available
4. **Save**

## Common Issues

### Issue: Material still pink after changing shader
**Solution:** 
- Check if textures are assigned
- Make sure you're using the correct shader for your render pipeline
- Try creating a new material from scratch

### Issue: Model has multiple materials, all pink
**Solution:**
- Select each material individually and fix them
- Or reimport the model with correct material settings

### Issue: Textures are missing
**Solution:**
- Check the imported asset folder for texture files
- They might be in a Textures subfolder
- Import them separately if needed
- Assign them to the material

## Quick Checklist

- [ ] Identified which materials are pink
- [ ] Checked render pipeline (Built-in or URP)
- [ ] Changed shader to appropriate one (Standard or URP Lit)
- [ ] Assigned textures if available
- [ ] Material no longer pink
- [ ] Model looks correct in Scene view

## Additional Tips

- **For Zombie Model:** Use URP Lit shader, assign textures
- **For Bow Model:** Use URP Lit shader, assign textures
- **For Environment:** Use URP Lit shader for consistency
- **Performance:** URP is better for mobile/iOS, so use URP Lit shader

