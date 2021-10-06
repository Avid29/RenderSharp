namespace System.Numerics
{
    public static class Vector3Extensions
    {
        public static float At(this Vector3 v3, int i)
        {
            return i switch
            {
                0 => v3.X,
                1 => v3.Y,
                2 => v3.Z,
                _ => 0,
            };
        }
    }
}
