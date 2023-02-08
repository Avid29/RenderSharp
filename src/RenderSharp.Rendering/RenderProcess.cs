// Adam Dernis 2023

using RenderSharp.Rendering.Enums;
using System;

namespace RenderSharp.Rendering;

public class RenderProcess
{
    public RenderProcess(string name, ProcessCategory category, TimeSpan start)
    {
        Name = name;
        Category = category;
        StartTime = start;
    }

    public string Name { get; }

    public ProcessCategory Category { get; }

    public TimeSpan StartTime { get; }

    public TimeSpan? EndTime { get; private set; }

    public TimeSpan? Duration => IsDone ? EndTime - StartTime : null;

    public bool IsDone => EndTime.HasValue;

    public void Finish(TimeSpan end)
    {
        EndTime = end;
    }
}
