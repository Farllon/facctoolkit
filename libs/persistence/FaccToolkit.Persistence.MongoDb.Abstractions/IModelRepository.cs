using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.MongoDb.Abstractions
{
    public interface IModelRepository<TModel>
        where TModel : class
    {
        Task<TModel?> FindByIdAsync<TId>(Expression<Func<TModel, TId>> idSelctor, TId id, CancellationToken cancellationToken);

        Task InsertAsync<TId>(Expression<Func<TModel, TId>> idSelctor, TModel entity, CancellationToken cancellationToken);

        Task InsertAsync<TId>(Expression<Func<TModel, TId>> idSelctor, IEnumerable<TModel> entities, CancellationToken cancellationToken);

        Task<TModel?> UpdateAsync<TId>(Expression<Func<TModel, TId>> idSelctor, TModel entity, CancellationToken cancellationToken);

        Task<TModel?> DeleteAsync<TId>(Expression<Func<TModel, TId>> idSelctor, TId id, CancellationToken cancellationToken);
    }
}
