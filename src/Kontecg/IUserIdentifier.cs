namespace Kontecg
{
    /// <summary>
    ///     Interface to get a user identifier.
    /// </summary>
    public interface IUserIdentifier
    {
        /// <summary>
        ///     Company Id. Can be null for host users.
        /// </summary>
        int? CompanyId { get; }

        /// <summary>
        ///     Id of the user.
        /// </summary>
        long UserId { get; }
    }
}
