// Copyright (c) 2014-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.

using System;

namespace SiliconStudio.Xenko.Engine
{
    /// <summary>
    /// Allows to declare that a component requires another component in order to run (used for <see cref="ScriptComponent"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequireComponentAttribute : EntityComponentAttributeBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RequireComponentAttribute"/>.
        /// </summary>
        /// <param name="type">Type of the required <see cref="EntityComponent"/></param>
        public RequireComponentAttribute(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Gets the type of the required component (Must be an <see cref="EntityComponent"/>.
        /// </summary>
        public Type Type { get; }
    }
}
