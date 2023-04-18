// Adam Dernis 2023

namespace RenderSharp.RayTracing.RayCasts;

public struct RayType
{
    // 0: Path
    // 1: Shadow
    public int rootType;

    // 0: Camera
    // 1: Diffuse
    // 2: Reflection
    // 3: Transmission
    public int secondaryType;
}
