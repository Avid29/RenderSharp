// Adam Dernis 2023

namespace RenderSharp.Rendering.Enums;

/// <summary>
/// An enum indicating the state of a renderer.
/// </summary>
public enum RenderState
{
#pragma warning disable CS1591
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
#pragma warning restore CS1591
}
