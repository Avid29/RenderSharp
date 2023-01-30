// Adam Dernis 2023

namespace RenderSharp.Rendering.Enums;

/// <summary>
/// An enum indicating the state of a renderer.
/// </summary>
public enum RenderState
{
    NotReady,
    Preparing,
    Ready,
    Running,
    Done,

    Cancelling,
    Cancelled,

    // TODO: Pausing
    //Paused,

    Error
}
