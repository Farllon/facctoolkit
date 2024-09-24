using FaccToolkit.Persistence.Abstractions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Persistence.MongoDb.Abstractions
{
    public abstract class MongoDocumentRepository<TModel, TId> : ModelRepository<TModel, TId>
        where TModel : class
        where TId : IEquatable<TId>
    {
        protected readonly IMongoDbContext _context;
        protected readonly IMongoCollection<TModel> _collection;

        public MongoDocumentRepository(ILogger logger, IMongoDbContext context, IMongoCollection<TModel> collection)
            : base(logger)
        {
            _context = context;
            _collection = collection;
        }

        protected override Task<TModel?> InternalFindByIdAsync(TId id, CancellationToken cancellationToken)
        {
            var queryFilter = GetQueryFilter();
            var idFilter = Builders<TModel>.Filter.Eq("_id", id);
            var filter = queryFilter is null
                ? idFilter
                : Builders<TModel>.Filter.And(
                    idFilter,
                    queryFilter);

            return _collection
                .Find(_context.CurrentSession, filter)
                .FirstOrDefaultAsync(cancellationToken)!;
        }

        protected override Task InternalInsertAsync(TModel model, CancellationToken cancellationToken)
            => _collection.InsertOneAsync(_context.CurrentSession, model, cancellationToken: cancellationToken);

        protected override Task InternalInsertAsync(IEnumerable<TModel> models, CancellationToken cancellationToken)
            => _collection.InsertManyAsync(_context.CurrentSession, models, cancellationToken: cancellationToken);

        protected override Task<TModel?> InternalUpdateAsync(TModel model, CancellationToken cancellationToken)
        {
            var id = GetId(model);
            var queryFilter = GetQueryFilter();
            var idFilter = Builders<TModel>.Filter.Eq("_id", id);
            var filter = queryFilter is null
                ? idFilter
                : Builders<TModel>.Filter.And(
                    idFilter,
                    queryFilter);

            return _collection
                .FindOneAndReplaceAsync(
                    _context.CurrentSession,
                    filter,
                    model,
                    cancellationToken: cancellationToken)!;
        }

        protected override Task<TModel?> InternalDeleteAsync(TId id, CancellationToken cancellationToken)
        {
            var queryFilter = GetQueryFilter();
            var idFilter = Builders<TModel>.Filter.Eq("_id", id);
            var filter = queryFilter is null
                ? idFilter
                : Builders<TModel>.Filter.And(
                    idFilter,
                    queryFilter);

            return _collection
                .FindOneAndDeleteAsync(
                    _context.CurrentSession,
                    filter,
                    cancellationToken: cancellationToken)!;
        }

        protected virtual FilterDefinition<TModel>? GetQueryFilter()
        {
            return null;
        }
    }
}