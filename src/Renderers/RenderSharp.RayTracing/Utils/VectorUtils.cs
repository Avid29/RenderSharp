// Adam Dernis 2023

namespace RenderSharp.RayTracing.Utils;

internal class VectorUtils
{
    public static float3 HSVtoRGB(float3 hsv)
    {
        float hf = hsv.X / 60;
        int i = (int)MathF.Floor(hf);
        float f = hf - i;
        float pv = hsv.Z * (1 - hsv.Y);
        float qv = hsv.Z * (1 - hsv.Y * f);
        float tv = hsv.Z * (1 - hsv.Y * (1 - f));

        switch (i)
        {
            case 0:
                return new float3(hsv.Z, tv, pv);
            case 1:
                return new float3(qv, hsv.Z, pv);
            case 2:
                return new float3(pv, hsv.Z, tv);
            case 3:
                return new float3(pv, qv, hsv.Z);
            case 4:
                return new float3(tv, pv, hsv.Z);
            case 5:
                return new float3(hsv.Z, pv, qv);
            case 6:
                return new float3(hsv.Z, tv, pv);
            default:
                return new float3(hsv.Z, pv, qv);
        }
    }
}
