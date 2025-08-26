using System.Collections.Generic;

namespace Kontecg.Updates
{
    /// <summary>
    ///     A specialized list to store <see cref="IUpdateSource" /> object.
    /// </summary>
    internal class UpdateSourceList : List<IUpdateSource>, IUpdateSourceList
    {
    }
}
