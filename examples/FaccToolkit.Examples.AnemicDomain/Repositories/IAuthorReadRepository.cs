using FaccToolkit.Domain.Anemic;
using FaccToolkit.Examples.AnemicDomain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Examples.AnemicDomain.Repositories
{
    public interface IAuthorReadRepository : IReadRepository<Author, Guid>
    {
        Task<IReadOnlyCollection<Author>> GetAllAsync(CancellationToken cancellationToken);
    }
}
