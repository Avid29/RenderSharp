// Adam Dernis 2023

using CommunityToolkit.Diagnostics;
using RenderSharp.Rendering.Enums;
using RenderSharp.Rendering.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace RenderSharp.Rendering;

public class RenderAnalyzer : IRenderAnalyzer
{
    private Stopwatch? _stopwatch;
    private List<RenderProcess> _processes;
    private RenderProcess? _activeProcess;

    public RenderAnalyzer()
    {
        _processes = new List<RenderProcess>();
    }

    [MemberNotNullWhen(true, nameof(_stopwatch))]
    public bool IsRunning => _stopwatch is not null && _stopwatch.IsRunning;

    public void Begin()
    {
        if (IsRunning)
            return;

        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }

    public void LogProcess(string name, ProcessCategory category)
    {
        if (!IsRunning)
            return;

        if (_activeProcess is not null)
            FinishProcess();

        _activeProcess = new RenderProcess(name, category, _stopwatch.Elapsed);
    }

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
