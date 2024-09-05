using FaccToolkit.Domain.Anemic;
using FaccToolkit.Examples.AnemicDomain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace FaccToolkit.Examples.AnemicDomain.Repositories
{
    public interface IAuthorRepository : IEntityRepository<Author, Guid>
    {
        Task<IReadOnlyCollection<Author>> GetAllAsync(CancellationToken cancellationToken);
    }
}
