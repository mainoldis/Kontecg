namespace Kontecg.Domain.Entities
{
    /// <summary>
    /// Implement this interface for an entity which must have CompanyId.
    /// </summary>
    public interface IMustHaveCompany
    {
        /// <summary>
        /// CompanyId of this entity.
        /// </summary>
        int CompanyId { get; set; }
    }
}
