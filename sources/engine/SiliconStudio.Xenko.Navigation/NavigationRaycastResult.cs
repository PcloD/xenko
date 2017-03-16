﻿// Copyright (c) 2016-2017 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using SiliconStudio.Core.Mathematics;

namespace SiliconStudio.Xenko.Navigation
{
    /// <summary>
    /// Result for a raycast query on a navigation mesh
    /// </summary>
    public struct NavigationRaycastResult
    {
        /// <summary>
        /// true if the raycast hit something
        /// </summary>
        public bool Hit;

        /// <summary>
        /// Position where the ray hit a non-walkable area boundary
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Normal of the non-walkable area boundary that was hit
        /// </summary>
        public Vector3 Normal;
    }
}