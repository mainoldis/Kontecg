using System;
using Kontecg.Collections.Extensions;

namespace Kontecg.Application.Navigation
{
    /// <summary>
    ///     Defines extension methods for <see cref="IHasMenuItemDefinitions" />.
    /// </summary>
    public static class HasMenuItemDefinitionsExtensions
    {
        /// <summary>
        ///     Searches and gets a <see cref="MenuItemDefinition" /> by it's unique name.
        ///     Throws exception if can not find.
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="name">Unique name of the source</param>
        public static MenuItemDefinition GetItemByName(this IHasMenuItemDefinitions source, string name)
        {
            MenuItemDefinition item = GetItemByNameOrNull(source, name);
            if (item == null)
            {
                throw new ArgumentException("There is no source item with given name: " + name, nameof(name));
            }

            return item;
        }

        /// <summary>
        ///     Searches all menu items (recursively) in the source and gets a <see cref="MenuItemDefinition" /> by it's unique
        ///     name.
        ///     Returns null if can not find.
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="name">Unique name of the source</param>
        public static MenuItemDefinition GetItemByNameOrNull(this IHasMenuItemDefinitions source, string name)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source.Items.IsNullOrEmpty())
            {
                return null;
            }

            foreach (MenuItemDefinition subItem in source.Items)
            {
                if (subItem.Name == name)
                {
                    return subItem;
                }

                MenuItemDefinition subItemSearchResult = GetItemByNameOrNull(subItem, name);
                if (subItemSearchResult != null)
                {
                    return subItemSearchResult;
                }
            }

            return null;
        }
    }
}
