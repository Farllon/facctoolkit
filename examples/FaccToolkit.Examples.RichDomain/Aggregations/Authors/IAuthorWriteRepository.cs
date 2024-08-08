using FaccToolkit.Domain.Rich;
using System;

namespace FaccToolkit.Examples.RichDomain.Aggregations.Authors
{
    public interface IAuthorWriteRepository : IWriteRepository<Author, Guid>
    {
        
    }
}
