// Adam Dernis 2023

using RenderSharp.Rendering.Enums;

namespace RenderSharp.Rendering.Interfaces;

public interface IRenderAnalyzer
{
    void LogProcess(string name, ProcessCategory category);
}
