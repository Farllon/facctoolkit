using FaccToolkit.Examples.MongoDb.RichDomainApi.Requests;
using FaccToolkit.Examples.RichDomain.Aggregations.Authors;
using Microsoft.AspNetCore.Mvc;

namespace FaccToolkit.Examples.MongoDb.RichDomainApi.Endpoints
{
    public static class AuthorEndpoint
    {
        public static IEndpointRouteBuilder MapAuthor(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("authors");

            group.MapGet("/", async (IAuthorReadRepository authorRepository, CancellationToken cancellationToken) =>
            {
                var authors = await authorRepository.GetAllAsync(cancellationToken);

                return authors.Count > 0
                    ? Results.Ok(authors)
                    : Results.NoContent();
            });

            group.MapPost("/", async ([FromBody] CreateEditAuthorRequest request, IAuthorWriteRepository authorRepository, CancellationToken cancelationToken) =>
            {
                var author = Author.Create(request.Name);

                await authorRepository.InsertAsync(author, cancelationToken);

                return Results.Created();
            });

            group.MapGet("{id:guid}", async ([FromRoute] Guid id, IAuthorReadRepository authorRepository, CancellationToken cancellationToken) =>
            {
                var author = await authorRepository.FindByIdAsync(id, cancellationToken);

                return author is null
                    ? Results.NotFound()
                    : Results.Ok(author);
            });

            group.MapPut("{id:guid}", async ([FromBody] CreateEditAuthorRequest request, [FromRoute] Guid id, IAuthorWriteRepository authorWriteRepository, IAuthorReadRepository authorReadRepository, CancellationToken cancellationToken) =>
            {
                var author = await authorReadRepository.FindByIdAsync(id, cancellationToken);

                if (author is null)
                    return Results.NotFound();

                author.SetName(request.Name);

                await authorWriteRepository.UpdateAsync(author, cancellationToken);

                return Results.Ok(author);
            });

            group.MapDelete("{id:guid}", async ([FromRoute] Guid id, IAuthorWriteRepository authorWriteRepository, IAuthorReadRepository authorReadRepository, CancellationToken cancellationToken) =>
            {
                var author = await authorReadRepository.FindByIdAsync(id, cancellationToken);

                if (author is null)
                    return Results.NotFound();

                await authorWriteRepository.DeleteAsync(author, cancellationToken);

                return Results.NoContent();
            });

            return builder;
        }
    }
}
