namespace RenderSharp.Scenes.Materials
{
    public abstract class MaterialBase
    {
        public MaterialBase(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
