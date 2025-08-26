﻿using System;

namespace Kontecg.Modules
{
    /// <summary>
    ///     Used to define dependencies of an Kontecg module to other modules.
    ///     It should be used for a class derived from <see cref="KontecgModule" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependsOnAttribute : Attribute
    {
        /// <summary>
        ///     Used to define dependencies of an Kontecg module to other modules.
        /// </summary>
        /// <param name="dependedModuleTypes">Types of depended modules</param>
        public DependsOnAttribute(params Type[] dependedModuleTypes)
        {
            DependedModuleTypes = dependedModuleTypes;
        }

        /// <summary>
        ///     Types of depended modules.
        /// </summary>
        public Type[] DependedModuleTypes { get; }
    }
}
