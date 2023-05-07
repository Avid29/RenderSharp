// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using RenderSharp.Rendering.Analyzer.Enums;
using RenderSharp.Rendering.Analyzer.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace RenderSharp.Rendering.Analyzer;

/// <summary>
/// A class for analyzing a render.
/// </summary>
public class RenderAnalyzer : IRenderAnalyzer
{
    private Stopwatch? _stopwatch;
    private List<RenderProcess> _processes;
    private RenderProcess? _activeProcess;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderAnalyzer"/> class.
    /// </summary>
    public RenderAnalyzer()
    {
        _processes = new List<RenderProcess>();
    }

    /// <summary>
    /// Get a value indicating whether or not the <see cref="RenderAnalyzer"/> is running.
    /// </summary>
    [MemberNotNullWhen(true, nameof(_stopwatch))]
    public bool IsRunning => _stopwatch is not null && _stopwatch.IsRunning;

    /// <summary>
    /// Begin the render analysis.
    /// </summary>
    public void Begin()
    {
        if (IsRunning)
            return;

        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }

    /// <inheritdoc/>
    public void LogProcess(string name, ProcessCategory category)
    {
        if (!IsRunning)
            return;

        if (_activeProcess is not null)
            FinishProcess();

        _activeProcess = new RenderProcess(name, category, _stopwatch.Elapsed);
    }

    /// <summary>
    /// Finish the render analysis.
    /// </summary>
    public void Finish()
    {
        if (!IsRunning)
            return;

        FinishProcess();
        _stopwatch.Stop();
    }

    private void FinishProcess()
    {
        Guard.IsNotNull(_stopwatch);
        Guard.IsNotNull(_activeProcess);

        _activeProcess.Finish(_stopwatch.Elapsed);
        _processes.Add(_activeProcess);
        _activeProcess = null;
    }
}
