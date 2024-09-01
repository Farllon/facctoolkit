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
        /// <summary>
        /// Search document by id
        /// </summary>
        /// <typeparam name="TId">The id type of document</typeparam>
        /// <param name="idSelctor">The document id property selector</param>
        /// <param name="id">The id to use in query</param>
        /// <param name="cancellationToken">The cancellation token to use in query</param>
        /// <returns>The found document if exists, otherwise returns null</returns>
        Task<TModel?> FindByIdAsync<TId>(Expression<Func<TModel, TId>> idSelctor, TId id, CancellationToken cancellationToken);

        /// <summary>
        /// Insert document in collection
        /// </summary>
        /// <typeparam name="TId">The id type of document</typeparam>
        /// <param name="idSelctor">The document id property selector</param>
        /// <param name="model">The model to insert</param>
        /// <param name="cancellationToken">The cancellation token to use in operation</param>
        /// <returns>The result of insertion operation</returns>
        Task InsertAsync<TId>(Expression<Func<TModel, TId>> idSelctor, TModel model, CancellationToken cancellationToken);

        /// <summary>
        /// Insert documents in collection
        /// </summary>
        /// <typeparam name="TId">The id type of document</typeparam>
        /// <param name="idSelctor">The document id property selector</param>
        /// <param name="models">The models to insert</param>
        /// <param name="cancellationToken">The cancellation token to use in operation</param>
        /// <returns>The result of insertion operation</returns>
        Task InsertAsync<TId>(Expression<Func<TModel, TId>> idSelctor, IEnumerable<TModel> models, CancellationToken cancellationToken);

        /// <summary>
        /// Find and update an document
        /// </summary>
        /// <typeparam name="TId">The id type of document</typeparam>
        /// <param name="idSelctor">The document id property selector</param>
        /// <param name="model">The model to update</param>
        /// <param name="cancellationToken">The cancellation token to use in operation</param>
        /// <returns>If document was found returns it before update, otherwise returs null</returns>
        Task<TModel?> UpdateAsync<TId>(Expression<Func<TModel, TId>> idSelctor, TModel model, CancellationToken cancellationToken);

        /// <summary>
        /// Find and delete an document
        /// </summary>
        /// <typeparam name="TId">The id type of document</typeparam>
        /// <param name="idSelctor">The document id property selector</param>
        /// <param name="id">The id of document</param>
        /// <param name="cancellationToken">The cancellation token to use in operation</param>
        /// <returns>The found document, otherwise returns null</returns>
        Task<TModel?> DeleteAsync<TId>(Expression<Func<TModel, TId>> idSelctor, TId id, CancellationToken cancellationToken);
    }
}
