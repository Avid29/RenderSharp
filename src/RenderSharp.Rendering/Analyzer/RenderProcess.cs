// Adam Dernis 2023

using RenderSharp.Rendering.Analyzer.Enums;
using System;

namespace RenderSharp.Rendering.Analyzer;

/// <summary>
/// A class for render analysis process render time tracking.
/// </summary>
public class RenderProcess
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RenderProcess"/> class.
    /// </summary>
    /// <param name="name">The name of the process.</param>
    /// <param name="category">The process category.</param>
    /// <param name="start">The start time of the practice relative to the render start time</param>
    public RenderProcess(string name, ProcessCategory category, TimeSpan start)
    {
        Name = name;
        Category = category;
        StartTime = start;
    }

    /// <summary>
    /// Gets the name of the process.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the process category.
    /// </summary>
    public ProcessCategory Category { get; }

    /// <summary>
    /// Gets the process start time relative to the render start time.
    /// </summary>
    public TimeSpan StartTime { get; }

    /// <summary>
    /// Gets the process end time relative to the render start time.
    /// </summary>
    public TimeSpan? EndTime { get; private set; }

    /// <summary>
    /// Gets the process duration.
    /// </summary>
    public TimeSpan? Duration => IsDone ? EndTime - StartTime : null;

    /// <summary>
    /// Gets a value indicating whether or not the process is done.
    /// </summary>
    public bool IsDone => EndTime.HasValue;

    /// <summary>
    /// Mark the process finished.
    /// </summary>
    /// <param name="end">The time the process ended.</param>
    public void Finish(TimeSpan end)
    {
        EndTime = end;
    }
}
