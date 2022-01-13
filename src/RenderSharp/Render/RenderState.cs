namespace RenderSharp.Render
{
    public enum RenderState
    {
        NotReady,
        Ready,
        Starting,
        Running,
        Done,
        // TODO: Cancel render
        //Cancelling,
        //Cancelled,
        Error,
    }
}
