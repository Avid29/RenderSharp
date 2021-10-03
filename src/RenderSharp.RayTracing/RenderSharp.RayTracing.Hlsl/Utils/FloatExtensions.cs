namespace ComputeSharp
{
    public static class FloatExtensions
    {
        public static float LengthSquared(this Float2 v2)
        {
            return v2.X * v2.X + v2.Y * v2.Y;
        }

        public static float LengthSquared(this Float3 v3)
        {
            return v3.X * v3.X + v3.Y * v3.Y + v3.Z * v3.Z;
        } 
    }
}
