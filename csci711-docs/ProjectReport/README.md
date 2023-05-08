# RenderSharp

Made by Adam Dernis for Global Illuminations at RIT (CSCI-711)

[Project Link](https://github.com/Avid29/RenderSharp)

## Results

Path traced ray tracing runs fully on the GPU.

Material shaders can be defined in external projects and registers with the RayTraceRenderer to allow any behavior desired.

Objects can be imported from obj into a mesh, or tesselated using various provided tesselation object types.

## Incomplete Components

Shadow rays do not reflect, refract, or gather attenuation resulting in an inaccurate shadow next to transparent objects.

After extensive debugging, both the BVH tree and multi-sampling are still incomplete.

Multi-sampling still has bands that appear to be the result of floating point issues.

The BVH Tree appears to either detect collisions wrong or build the tree wrong, however everytime I make a small and humanly parsable tree it works. More complex scenes give errors though.

## Project Components

- Wrapping Infrastructure
  - [x] Scenes can be built with a more simple object/mesh structure before being deployed to the GPU for rendering.
  - [ ] Render Manager
    - [x] Realtime renderer that renderer frames directly to the display buffer.
    - [x] Non-realtime renderer that renderers frames to a buffer and copies progress to the display buffer while rendering
      - [x] Renders in segments
      - [x] Supports cancellation
      - [ ] Can save result to jpeg image file
  - [ ] Render Analyzer
    - [x] Track time spent:
      - [x] Setting up
      - [x] Building BVH Tree
      - [x] Rendering
    - [ ] Output interpretable data
  - [x] Renderer Interface
    - [x] Allow a ray tracing renderer
    - [ ] Allow a wireframe renderer.
    - [ ] Allow the use of a rasterization renderer
      - [ ] Used for 3D chess game
        - [x] Chess Engine
- [x] Ray Tracer
  - [x] Camera Ray Caster
    - [x] Emit rays from camera with perspective
    - [x] Runs on GPU
  - [x] Shadow Ray Casting
    - [x] Emits shadow rays at collision point
    - [x] Runs on GPU
  - [x] Geometry Collision Detection
    - [x] Determine nearest geometry collision in path
    - [x] Track collided geometry material
    - [x] Calculate barycentric collision position
    - [x] Runs on GPU
    - [x] Speed up with spatial data structure*
    - [ ] Hardware accelerate with TerraFX interrupts
  - [x] Multi-Sampling*
  - [x] Material Shaders
    - [x] Implement shading model(s)
      - [x] Diffusion
      - [x] Reflection
      - [x] Transmission
      - [x] Principled
    - [x] Smooth Shading
    - [x] Run on GPU
  - [x] Object Loader
    - [x] Converts objects from common scene infrasture to renderable objects.
    - [x] Builds a BVH tree with the loaded objects*
    - [ ] GPU Assisted*

###### *  Marked components have known major bugs