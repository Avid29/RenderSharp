namespace RenderSharp.RayTracing.CPU.Scenes
{
    public struct RayTracingConfig
    {
        public RayTracingConfig(int samples, int bounces)
        {
            Samples = samples;
            MaxBounces = bounces;
        }

        public int Samples { get; }

        public int MaxBounces { get; }
    }
}
