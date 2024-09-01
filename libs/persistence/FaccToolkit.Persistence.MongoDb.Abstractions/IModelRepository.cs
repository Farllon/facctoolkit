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
        /// Searches for a document in the collection using the specified identifier.
        /// </summary>
        /// <typeparam name="TId">The type of the document identifier.</typeparam>
        /// <param name="idSelector">An expression to select the identifier property of the document.</param>
        /// <param name="id">The identifier value to search for.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the found document, or null if no document is found.</returns>
        Task<TModel?> FindByIdAsync<TId>(Expression<Func<TModel, TId>> idSelector, TId id, CancellationToken cancellationToken);

        /// <summary>
        /// Inserts a new document into the collection.
        /// </summary>
        /// <typeparam name="TId">The type of the document identifier.</typeparam>
        /// <param name="idSelector">An expression to select the identifier property of the document.</param>
        /// <param name="model">The document to insert.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous insertion operation.</returns>
        Task InsertAsync<TId>(Expression<Func<TModel, TId>> idSelector, TModel model, CancellationToken cancellationToken);

        /// <summary>
        /// Inserts multiple new documents into the collection.
        /// </summary>
        /// <typeparam name="TId">The type of the document identifier.</typeparam>
        /// <param name="idSelector">An expression to select the identifier property of the documents.</param>
        /// <param name="models">A collection of documents to insert.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous insertion operation.</returns>
        Task InsertAsync<TId>(Expression<Func<TModel, TId>> idSelector, IEnumerable<TModel> models, CancellationToken cancellationToken);

        /// <summary>
        /// Finds a document by its identifier and updates it with the provided model data.
        /// </summary>
        /// <typeparam name="TId">The type of the document identifier.</typeparam>
        /// <param name="idSelector">An expression to select the identifier property of the document.</param>
        /// <param name="model">The model data to update the document with.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the document before the update, or null if no document is found.</returns>
        Task<TModel?> UpdateAsync<TId>(Expression<Func<TModel, TId>> idSelector, TModel model, CancellationToken cancellationToken);

        /// <summary>
        /// Finds a document by its identifier and deletes it from the collection.
        /// </summary>
        /// <typeparam name="TId">The type of the document identifier.</typeparam>
        /// <param name="idSelector">An expression to select the identifier property of the document.</param>
        /// <param name="id">The identifier of the document to delete.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the deleted document, or null if no document is found.</returns>
        Task<TModel?> DeleteAsync<TId>(Expression<Func<TModel, TId>> idSelector, TId id, CancellationToken cancellationToken);
    }
}