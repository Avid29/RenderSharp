# Settings the scene

This image was rendered in Blender.

![image](SettingTheScene.png)

## Objects

- Camera at ``<0, 0, 1>`` with a ``75°`` field of view and looking at ``<0, 2, 1>`` (by tracking to an empty object)
- Glass sphere at ``<0, 2, 1>`` with a radius of ``0.5m`` and a thickness of ``1cm``
- Glossy sphere at ``<-0.75, 2.5, 0.5>`` with a radius of ``0.4m``
- Point light at ``<0.5, 0.5, 2.5>`` with a power of ``10w`` and a ``0.25m`` radius
- Tiled floor centered at ``<-1.25, 4, 0>`` with a width of ``5.5m`` and a depth of ``8m``

## Shaders

- The glass shader has a roughness of ``0``, an IOR of ``1.45``, and a pure white albedo
- The glossy shader has a roughness of ``0.25`` and a pure white albedo
- The tiled floor shader is a pure diffuse with a roughness of ``1`` and a checkered texture for the albedo

The spheres are UV spheres with ``32`` segments and ``16`` rings

## Other details

They sky is pure white with a strength of ``0.5``, however the image is rendered with a transparent sky, then overlayed with Alpha Over compositing on a texture of color ``#0089BC`` a.k.a. ``rgb(0, 0.25, 0.5)``.

The .blend file is available [here](SettingTheScene.blend)

Adam Dernis
