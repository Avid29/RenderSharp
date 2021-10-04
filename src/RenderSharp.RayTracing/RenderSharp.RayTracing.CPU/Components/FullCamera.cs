using RenderSharp.RayTracing.CPU.Rays;
using RenderSharp.RayTracing.CPU.Utils;
using System;
using System.Numerics;

namespace RenderSharp.RayTracing.CPU.Components
{
    public struct FullCamera
    {
        private Vector3 _horizontal;
        private Vector3 _vertical;
        private Vector3 _lowerLeftCorner;
        private Vector3 _u, _v, _w;
        private float _lensRadius;

        public FullCamera(Camera specs, float aspectRatio)
        {
            float theta = FloatUtils.DegreesToRadians(specs.FOV);
            float h = MathF.Tan(theta / 2);
            float height = 2 * h;
            float width = aspectRatio * height;

            Vector3 vup = Vector3.UnitY;

            Origin = specs.Origin;
            _w = Vector3.Normalize(specs.Origin - specs.Look);
            _u = Vector3.Normalize(Vector3.Cross(vup, _w));
            _v = Vector3.Cross(_w, _u);
            _horizontal = width * _u;
            _vertical = height * _v;

            Vector3 depth = _w * specs.FocalLength;

            _lowerLeftCorner = Origin - _horizontal / 2 - _vertical / 2 - depth;
            _lensRadius = specs.Aperture / 2;
        }

        public Ray CreateRay(float u, float v, ref uint randState)
        {
            Vector3 rand = _lensRadius * RandUtils.RandomInUnitDisk(ref randState);
            Vector3 offset = _u * rand.X + _v * rand.Y;

            Vector3 direction = _lowerLeftCorner + u * _horizontal + v * _vertical - Origin;
            return new Ray(Origin + offset, direction - offset);
        }

        public Vector3 Origin { get; }
    }
}
