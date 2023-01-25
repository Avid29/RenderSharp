// Adam Dernis 2023

namespace RenderSharp.Rendering;

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
