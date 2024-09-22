using FaccToolkit.Persistence.Abstractions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.MongoDb.Abstractions
{
    public class MongoDocumentRepository<TModel> : ModelRepository<TModel>
        where TModel : class
    {
        protected readonly IMongoDbContext _context;
        protected readonly IMongoCollection<TModel> _collection;

        public MongoDocumentRepository(ILogger logger, IMongoDbContext context, IMongoCollection<TModel> collection)
            : base(logger)
        {
            _context = context;
            _collection = collection;
        }

        protected override Task<TModel?> InternalFindByIdAsync<TId>(Func<TModel, TId> idSelector, TId id, CancellationToken cancellationToken)
            => _collection
                .Find(_context.CurrentSession, GetFilter(model => idSelector(model)!.Equals(id)))
                .FirstOrDefaultAsync(cancellationToken)!;

        protected override Task InternalInsertAsync(TModel model, CancellationToken cancellationToken)
            => _collection.InsertOneAsync(_context.CurrentSession, model, cancellationToken: cancellationToken);

        protected override Task InternalInsertAsync(IEnumerable<TModel> models, CancellationToken cancellationToken)
            => _collection.InsertManyAsync(_context.CurrentSession, models, cancellationToken: cancellationToken);

        protected override Task<TModel?> InternalUpdateAsync<TId>(Func<TModel, TId> idSelector, TModel model, CancellationToken cancellationToken)
        {
            var id = idSelector(model);

            return _collection
                .FindOneAndReplaceAsync(
                    _context.CurrentSession,
                    GetFilter(model => idSelector(model)!.Equals(id)),
                    model,
                    cancellationToken: cancellationToken)!;
        }

        protected override Task<TModel?> InternalDeleteAsync<TId>(Func<TModel, TId> idSelector, TId id, CancellationToken cancellationToken)
            => _collection
                    .FindOneAndDeleteAsync(
                        _context.CurrentSession,
                        GetFilter(model => idSelector(model)!.Equals(id)),
                        cancellationToken: cancellationToken)!;
    }
}