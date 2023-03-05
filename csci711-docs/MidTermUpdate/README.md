# RenderSharp

Made by Adam Dernis for Global Illuminations at RIT (CSCI-711)

[Project Link](https://github.com/Avid29/RenderSharp)

## Progress

Super Sampling has been added to the project components list, which is important because I plan on using sampling instead of branched execution (so I'll be path tracing). However, I believe my sampling system is currently giving me floating errors so that will need to be fixed. I'm also looking for a good way to generate random numbers, which will be super important when there are multiple samples.

I'm also struggling with traversing my BVH tree. I truly do not see the issue. I plan on writing unit tests to get to the bottom of it.

## Project Components

- Wrapping Infrastructure
  - [x] Scenes can be built with a more simple object/mesh structure before being deployed to the GPU for rendering.
  - [ ] Render Manager
    - [x] Realtime renderer that renderer frames directly to the display buffer.
    - [x] Non-realtime renderer that renderers frames to a buffer and copies progress to the display buffer while rendering
      - [x] Renders in segments
      - [ ] Supports cancellation*
      - [ ] Can save result to jpeg image file*
  - [ ] Render Analyzer
    - [x] Track time spent:
      - [x] Setting up
      - [x] Building BVH Tree
      - [x] Rendering
    - [ ] Output interpretable data
  - [x] Renderer Interface
    - [x] Allow a ray tracing renderer
    - [ ] Allow a wireframe renderer.*
    - [ ] Allow the use of a rasterization renderer*
      - [ ] Used for 3D chess game if needed

- [ ] Ray Tracer
  - [x] Camera Ray Caster
    - [x] Emit rays from camera with perspective
    - [x] Runs on GPU
  - [x] Geometry Collision Detection
    - [x] Determine nearest geometry collision in path
    - [x] Track collided geometry material
    - [x] Calculate barycentric collision position
    - [x] Runs on GPU
    - [ ] Speed up with spatial data structure*
  - [ ] Super Sampling
  - [ ] Material Shaders
    - [ ] Implement shading model(s)
      - [x] Diffusion
      - [ ] Reflection
      - [ ] Transmission
      - [ ] Emission*
      - [ ] Subsurface Scattering*
    - [x] Run on GPU
  - [x] Object Loader
    - [x] Converts objects from common scene infrasture to renderable objects.
    - [x] Builds a BVH tree with the loaded objects*
    - [ ] GPU Assisted*
  - [ ] Mesh Transformer*
    - [ ] Updates transformations on all vertices belonging to an object.
    - [ ] Update BVH trees instead of entirely constructing.*
    - [ ] GPU Assisted*
  - [ ] Sobol Pattern Generation*
    - [ ] Some form of randomization pattern will be needed for sampling, and sobol seems desirable, however if I find a better method or don't have time and find an easier method that maybe use that instead

###### *  Marked components are not a priority and may not be completed
