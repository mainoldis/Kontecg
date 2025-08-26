namespace Kontecg.Domain.Entities
{
    /// <summary>
    /// Implement this interface for an entity which may optionally have CompanyId.
    /// </summary>
    public interface IMayHaveCompany
    {
        /// <summary>
        /// CompanyId of this entity.
        /// </summary>
        int? CompanyId { get; set; }
    }
}
