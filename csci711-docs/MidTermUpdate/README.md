# RenderSharp

Made by Adam Dernis for Global Illuminations at RIT (CSCI-711)

[Project Link](https://github.com/Avid29/RenderSharp)

## Progress

## Project Components

- Wrapping Infrastructure
  - [x] Scenes can be built with a more simple object/mesh structure before being deployed to the GPU for rendering.

  - [ ] Render Manager
    - [x] Realtime renderer that renderer frames directly to the display buffer.
    - [ ] Non-realtime renderer that renderers frames to a buffer and copies progress to the display buffer while rendering
      - [x] Renders in segments
      - [ ] Supports cancellation*
      - [ ] Can save result to jpeg image file*

  - [ ] Renderer Interface
    - [x] Allow a ray tracing renderer
    - [ ] Allow a wireframe renderer.*
    - [ ] Allow the use of a rasterization renderer*
      - [ ] Used for 3D chess game if needed

- Ray Tracer
  - [x] Camera Ray Caster
    - [x] Emit rays from camera with perspective
    - [x] Runs on GPU
  - [x] Geometry Collision Detection
    - [x] Determine nearest geometry collision in path
    - [x] Track collided geometry material
    - [x] Calculate barycentric collision position
    - [x] Runs on GPU
    - [ ] Speed up with spatial data structure*
  - [ ] Material Shaders
    - [ ] Implement shading model(s)
      - [x] Diffusion
      - [ ] Reflection
      - [ ] Transmission
      - [ ] Emission*
      - [ ] Subsurface Scattering*
    - [x] Run on GPU
  - [ ] Object Loader
    - [x] Converts objects from common scene infrasture to renderable objects.
    - [x] Builds a BVH tree with the loaded objects*
    - [ ] GPU Assisted*
  - Mesh Transformer*
    - [ ] Updates transformations on all vertices belonging to an object.
    - [ ] Update BVH trees instead of entirely constructing.*
    - [ ] GPU Assisted*
  - Sobol Pattern Generation*
    - [ ] Some form of randomization pattern will be needed for sampling, and sobol seems desirable, however if I find a better method or don't have time and find an easier method that maybe use that instead

###### *  Marked components are not a priority and may not be completed
