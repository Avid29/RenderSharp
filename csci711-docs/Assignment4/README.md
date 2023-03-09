# Procedural Shading

![image](Checkers.png)

This image was rendered in RenderSharp.

# Radial Gradient

![image](RadialGradient.png)

A radial gradient texture has replaced the checkers

# Object Space

![image](ObjectSpace.png)

The texture space has been transformed to the object space instead of the world space

# HSV Radial Gradient Texture

![image](RadialHSVGradient.png)

The UnitY to UnitX colors have been replaced with an HSV wheel following this equation

```cs
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
```
Adam Dernis
