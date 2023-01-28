// Adam Dernis 2023

namespace RenderSharp.RayTracing.Utils;

internal class VectorUtils
{
    /// <summary>
    /// Converts an HSV <see cref="float3"/> into an RGB <see cref="float3"/>.
    /// </summary>
    /// <remarks>
    /// HSV <see cref="float3"/> where:<br/>
    /// X is a hue between 0 and 360.<br/>
    /// Y is a saturation between 0 and 1.<br/>
    /// Z is a value between 0 and 1.<br/>
    /// </remarks>
    /// <param name="hsv">The HSV <see cref="float3"/>.</param>
    /// <returns>The RGB <see cref="float3"/>.</returns>
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
