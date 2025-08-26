using System;

namespace Kontecg.Application.Services.Dto
{
    /// <summary>
    ///     Simply implements <see cref="IPagedAndSortedResultRequest" />.
    /// </summary>
    [Serializable]
    public class PagedAndSortedResultRequestDto : PagedResultRequestDto, IPagedAndSortedResultRequest
    {
        public virtual string Sorting { get; set; }
    }
}
