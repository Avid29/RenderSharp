﻿// Adam Dernis 2023

namespace RenderSharp.RayTracing;

/// <summary>
/// A struct containing ray tracing render configuration settings.
/// </summary>
public struct RayTracingConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RayTracingConfig"/> struct.
    /// </summary>
    public RayTracingConfig()
    {
        // Default config
        SampleCount = 1;
        UseBVH = false;
        MaxBounceDepth = 8;
    }

    /// <summary>
    /// Gets or sets the number of samples to take per pixel.
    /// </summary>
    public int SampleCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not to use BVH optimization for collision detection optimization.
    /// </summary>
    public bool UseBVH { get; set; }

    /// <summary>
    /// Gets or sets the max number of times rays should bounce .
    /// </summary>
    public int MaxBounceDepth { get; set; }
}
