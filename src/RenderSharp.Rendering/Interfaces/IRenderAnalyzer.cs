// Adam Dernis 2023

using RenderSharp.Rendering.Enums;

namespace RenderSharp.Rendering.Interfaces;

/// <summary>
/// An interface for a exposing the <see cref="RenderAnalyzer"/> to an <see cref="IRenderer"/>.
/// </summary>
public interface IRenderAnalyzer
{
    /// <summary>
    /// Log a new rendering process.
    /// </summary>
    /// <param name="name">The name of the process.</param>
    /// <param name="category">The process category.</param>
    void LogProcess(string name, ProcessCategory category);
}
