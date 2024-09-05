using FaccToolkit.Examples.AnemicDomain.Entities;
using FaccToolkit.Examples.AnemicDomain.Repositories;
using FaccToolkit.Examples.MongoDb.AnemicDomainApi.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FaccToolkit.Examples.MongoDb.AnemicDomainApi.Endpoints
{
    public static class AuthorEndpoint
    {
        public static IEndpointRouteBuilder MapAuthor(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("authors");

            group.MapGet("/", async (IAuthorRepository authorRepository, CancellationToken cancellationToken) =>
            {
                var authors = await authorRepository.GetAllAsync(cancellationToken);

                return authors.Count > 0
                ? Results.Ok(authors)
                : Results.NoContent();
            });

            group.MapPost("/", async ([FromBody] CreateEditAuthorRequest request, IAuthorRepository authorRepository, CancellationToken cancelationToken) =>
            {
                var author = new Author(request.Name);

                await authorRepository.InsertAsync(author, cancelationToken);

                return Results.Created();
            });

            group.MapGet("{id:guid}", async ([FromRoute] Guid id, IAuthorRepository authorRepository, CancellationToken cancellationToken) =>
            {
                var author = await authorRepository.FindByIdAsync(id, cancellationToken);

                return author is null
                    ? Results.NotFound()
                    : Results.Ok(author);
            });

            group.MapPut("{id:guid}", async ([FromBody] CreateEditAuthorRequest request, [FromRoute] Guid id, IAuthorRepository authorRepository, CancellationToken cancellationToken) =>
            {
                var author = new Author(id, request.Name);

                await authorRepository.UpdateAsync(author, cancellationToken);

                return Results.Ok(author);
            });

            group.MapDelete("{id:guid}", async ([FromRoute] Guid id, IAuthorRepository authorRepository, CancellationToken cancellationToken) =>
            {
                await authorRepository.DeleteAsync(id, cancellationToken);

                return Results.NoContent();
            });

            return builder;
        }
    }
}
