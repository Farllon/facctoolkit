using FaccToolkit.Domain.Anemic;
using FaccToolkit.Examples.AnemicDomain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Examples.AnemicDomain.Repositories
{
    public interface IPostReadRepository : IReadRepository<Post, Guid>
    {
        Task<IReadOnlyCollection<Post>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken);
    }
}
