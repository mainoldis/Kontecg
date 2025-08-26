namespace Kontecg.Authorization
{
    /// <summary>
    ///     Represents a permission <see cref="Name" /> with <see cref="IsGranted" /> information.
    /// </summary>
    public class PermissionGrantInfo
    {
        /// <summary>
        ///     Creates a new instance of <see cref="PermissionGrantInfo" />.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isGranted"></param>
        public PermissionGrantInfo(string name, bool isGranted)
        {
            Name = name;
            IsGranted = isGranted;
        }

        /// <summary>
        ///     Name of the permission.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Is this permission granted Prohibited?
        /// </summary>
        public bool IsGranted { get; }
    }
}
