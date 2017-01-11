﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;
using SiliconStudio.Core;

namespace SiliconStudio.Xenko.Rendering.Composers
{
    /// <summary>
    /// Identifies a camera slotIndex in a scene composition.
    /// </summary>
    [DataContract("SceneCameraSlotIndex")]
    [DataStyle(DataStyle.Compact)]
    public struct SceneCameraSlotIndex : IEquatable<SceneCameraSlotIndex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SceneCameraSlotIndex"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        public SceneCameraSlotIndex(int index)
        {
            Index = index;
        }

        /// <summary>
        /// Index of the camera in <see cref="SceneGraphicsCompositorLayers.Cameras"/>
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// Performs an implicit conversion from <see cref="SceneCameraSlotIndex"/> to <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="slotIndex">The slotIndex.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator int(SceneCameraSlotIndex slotIndex)
        {
            return slotIndex.Index;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int32"/> to <see cref="SceneCameraSlotIndex"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator SceneCameraSlotIndex(int index)
        {
            return new SceneCameraSlotIndex(index);
        }

        public override string ToString()
        {
            return $"Cameras[{Index}]";
        }

        public bool Equals(SceneCameraSlotIndex other)
        {
            return Index == other.Index;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SceneCameraSlotIndex && Equals((SceneCameraSlotIndex)obj);
        }

        public override int GetHashCode()
        {
            return Index.GetHashCode();
        }

        public static bool operator ==(SceneCameraSlotIndex left, SceneCameraSlotIndex right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SceneCameraSlotIndex left, SceneCameraSlotIndex right)
        {
            return !left.Equals(right);
        }
    }
}