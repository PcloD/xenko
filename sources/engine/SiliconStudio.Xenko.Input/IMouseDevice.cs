// Copyright (c) 2016-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.

using SiliconStudio.Core.Collections;
using SiliconStudio.Core.Mathematics;

namespace SiliconStudio.Xenko.Input
{
    /// <summary>
    /// Represents functionality specific to mouse input such as buttons, wheels, mouse locking and setting cursor position
    /// </summary>
    public interface IMouseDevice : IPointerDevice
    {
        /// <summary>
        /// Normalized position(0,1) of the mouse
        /// </summary>
        Vector2 Position { get; }

        /// <summary>
        /// Mouse delta
        /// </summary>
        Vector2 Delta { get; }
        
        /// <summary>
        /// The mouse buttons that have been pressed since the last frame
        /// </summary>
        IReadOnlySet<MouseButton> PressedButtons { get; }

        /// <summary>
        /// The mouse buttons that have been released since the last frame
        /// </summary>
        IReadOnlySet<MouseButton> ReleasedButtons { get; }

        /// <summary>
        /// The mouse buttons that are down
        /// </summary>
        IReadOnlySet<MouseButton> DownButtons { get; }
        
        /// <summary>
        /// Gets or sets if the mouse is locked to the screen
        /// </summary>
        bool IsPositionLocked { get; }

        /// <summary>
        /// Locks the mouse position to the screen
        /// </summary>
        /// <param name="forceCenter">Force the mouse position to the center of the screen</param>
        void LockPosition(bool forceCenter = false);

        /// <summary>
        /// Unlocks the mouse position if it was locked
        /// </summary>
        void UnlockPosition();

        /// <summary>
        /// Attempts to set the pointer position, this only makes sense for mouse pointers
        /// </summary>
        /// <param name="normalizedPosition"></param>
        void SetPosition(Vector2 normalizedPosition);
    }
}