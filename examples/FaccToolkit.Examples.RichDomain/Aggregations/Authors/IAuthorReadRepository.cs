using FaccToolkit.Domain.Rich;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Examples.RichDomain.Aggregations.Authors
{
    public interface IAuthorReadRepository : IReadRepository<Author, Guid>
    {
        Task<IReadOnlyCollection<Author>> GetAllAsync(CancellationToken cancellationToken);
    }
}
