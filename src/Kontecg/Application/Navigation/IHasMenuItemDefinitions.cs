﻿using System.Collections.Generic;

namespace Kontecg.Application.Navigation
{
    /// <summary>
    ///     Declares common interface for classes those have menu items.
    /// </summary>
    public interface IHasMenuItemDefinitions
    {
        /// <summary>
        ///     List of menu items.
        /// </summary>
        List<MenuItemDefinition> Items { get; }
    }
}
