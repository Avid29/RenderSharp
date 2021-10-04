namespace System.Numerics
{
    public static class Vector3Extensions
    {
        public static float At(this Vector3 v3, int i)
        {
            return i switch
            {
                1 => v3.X,
                2 => v3.Y,
                3 => v3.Z,
                _ => 0,
            };
        }
    }
}
