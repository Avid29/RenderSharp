using RenderSharp.Common.Scenes.Materials;

namespace RenderSharp.Common.Scenes.Objects
{
    /// <summary>
    /// A common interface between classes representing renderable objects.
    /// </summary>
    public interface IObject
    {
        public IMaterial Material { get; }
    }
}
