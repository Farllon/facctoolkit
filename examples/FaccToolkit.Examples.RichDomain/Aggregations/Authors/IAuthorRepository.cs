using FaccToolkit.Domain.Rich;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace FaccToolkit.Examples.RichDomain.Aggregations.Authors
{
    public interface IAuthorRepository : IAggregateRepository<Author, Guid>
    {
        Task<IReadOnlyCollection<Author>> GetAllAsync(CancellationToken cancellationToken);
    }
}
