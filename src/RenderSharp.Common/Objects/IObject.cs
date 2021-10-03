﻿using RenderSharp.Common.Materials;

namespace RenderSharp.Common.Objects
{
    /// <summary>
    /// A common interface between classes representing renderable objects.
    /// </summary>
    public interface IObject
    {
        public IMaterial Material { get; }
    }
}
