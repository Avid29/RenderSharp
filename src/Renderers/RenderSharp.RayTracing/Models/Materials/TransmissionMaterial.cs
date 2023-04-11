// Adam Dernis 2023

namespace RenderSharp.RayTracing.Models.Materials;

public struct TransmissionMaterial
{
    public float ior;

    public TransmissionMaterial(float ior)
    {
        this.ior = ior;
    }
}
