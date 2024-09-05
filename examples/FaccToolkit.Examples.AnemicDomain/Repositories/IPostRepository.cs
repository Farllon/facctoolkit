using FaccToolkit.Domain.Anemic;
using FaccToolkit.Examples.AnemicDomain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace FaccToolkit.Examples.AnemicDomain.Repositories
{
    public interface IPostRepository : IEntityRepository<Post, Guid>
    {
        Task<IReadOnlyCollection<Post>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken);
    }
}
