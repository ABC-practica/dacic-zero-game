# Scene & Post-Processing Shader Documentation

## 0.Overview

This document covers the current playable scene and the custom camera shader used to simulate Gabi's wanted style look.


## 1. Scene Composition

### Terrain

-   Built using Unity's built-in Terrain system with hand-painted textures.
-   Terrain textures are painted directly in the Terrain component using "Paint texture".

### Trees

-   Trees are painted onto the terrain using the Terrain tree painting tool.
-   A single tree prefab is currently assigned as the paintable tree asset.
-   Tree prefab is sourced from the Unity Asset Store (free asset).
-   For future levels use the "Mass Place Trees" button with your desired density
-   The tree painting tool is not just for trees it can work with any prefab to scatter models onto the terrain

### Navigation for Bots

-   A NavMesh Surface is baked over the terrain to support AI pathfinding.
-   Enemies use NavMeshAgent components to path across the baked surface.
-   Any terrain edits require re-baking the NavMesh Surface so that everything works.

### Player Camera

-   The DitherEffect post-processing script is attached directly to the player's camera.

### Assets
- Assets are free rom the Unity  Asset store.
- Go to Window->Package Manager-> My Assets to find all the current and future assets you will use



## 2. DitherEffect (Camera Post-Processing Shader)

### Purpose

- Simulates the black and white, blue-noise dithered visual style of Return of the Obra Dinn game  as a full-screen camera post-process.
-  Applied to the first-person player camera.

### How It Works

-   Implements Unity's legacy OnRenderImage post-processing pipeline "RequireComponent(typeof(Camera))", so it requires the built-in render pipeline not URP/HDRP. 
(keep this in mind if you switch to URP for any reason it might break the rendering. For more rendering details you can go to Window->Rendering->Lighting)

-   "ExecuteAlways" lets the effect preview in the Editor, not just Play mode.
-   On enable, it auto locates the Hidden/ObraDither shader, and instantiates a runtime only Material "HideFlags.HideAndDontSave" from it, the material is destroyed on disable to avoid leaks.
-   Each frame, the script tells the shader what settings to use, then feeds the camera's normal image through that shader to produce the dithered image you actually see.(pretty cool if i do say so myself)

### Rendering things

The effect has three mutually exclusive rendering paths, chosen based on  useSupersampling or useLowRes

1.  Supersampling (useSupersampling = true) Renders the dither pass into a temporary RenderTexture larger than screen size (by supersampleFactor), then downsamples to screen.  The dither dots look more hd, more crisp, less chunk.
    
2.  LowRes Downsample (useLowRes = true, supersampling off) Renders the dither pass into a smaller temporary RenderTexture (resolutionScale) using point filtering, then upscales. Produces a chunkier, blockier look.
    
3.  Standard (neither flag set) Dither at native resolution.

These things are the consequences of redoing the shader again and the again to keep the effect looking good while still having good visibility. Might be more complicated then needed but it works.

### Inspector Parameters

- Dither Shader: Auto-found via Hidden/ObraDither if left empty.

- Noise Tex: Blue-noise texture used to drive the dither pattern.  Use: BlueNoiseTexture64x64
	(Use BayerTexture8x8 for a different look)

- Dark Color: Color mapped to darker luminance values. Default: black

- Light Color: Color mapped to brighter luminance values. Default: white

- Noise Scale: Scale of the noise texture across the screen, higher values = denser dither dots. Default: 4, Use: 5

- Softness: Controls how soft/gradual the transition is between dark and light dither regions. Default: 0.03 Use: 0.2

- Min Luminance: clamps how dark the image can get before dithering, keeping shadows from going to pure black. Default: 0 Use: 0.32

- Contrast: Contrast multiplier applied to luminance before dithering. Default: 3 Use: 1.02

- Dither Amount: Blends between the original image and the fully dithered result, low values keep the effect subtle. Default: 0.5 Use: 0.054 (do you want to be able to understand the screen? gut! dont change this)

- Use Supersampling: Enables the supersampled (fine-dot) rendering path.

- Supersample Factor: Multiplier for render resolution. Default: 2 Use: 3.33

- Use Low Res: Enables the low-res rendering path.

- Resolution Scale: Fraction of screen resolution to render at. Default: 0.5

### Current Values

The player camera currently uses a high contrast suppressed, fine detail configuration:
-  low contrast and the low dither amount keeps the effect subtle
-  supersampling at 3.33x keeps the dither dots crisp despite the softened settings
-  Min luminance of 0.32 prevents shadows from going fully black

In plain english: Looks good, the effect is crisp but does not diminish visibility for gameplay

### Important things

1.  Project must use the built-in render pipeline
2.  Requires the Hidden/ObraDither shader to exist in the project.(do not remove or i will break your ankles :D)
3.  Requires a blue-noise texture assigned to Noise Tex.
4.  Attach DitherEffect to any Camera component.(i am not sure if the player prefab has it by default)

### Other things

-   ExecuteAlways means the effect also runs in the Scene/Game view in Edit mode, which is useful for tuning things outside Play mode.(it is not 100% needed but i like to work like that)