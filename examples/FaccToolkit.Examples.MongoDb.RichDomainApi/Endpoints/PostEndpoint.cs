using FaccToolkit.Examples.MongoDb.RichDomainApi.Requests;
using FaccToolkit.Examples.RichDomain.Aggregations.Authors;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace FaccToolkit.Examples.MongoDb.RichDomainApi.Endpoints
{
    public static class PostEndpoint
    {
        public static IEndpointRouteBuilder MapPost(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("{authorId}/posts");

            group.MapGet("/", async ([FromRoute] Guid authorId, IAuthorRepository authorRepository, CancellationToken cancellationToken) =>
            {
                var author = await authorRepository.FindByIdAsync(authorId, cancellationToken);

                if (author is null)
                    return Results.NotFound();

                return author.Posts.Count > 0
                    ? Results.Ok(author.Posts)
                    : Results.NoContent();
            });

            group.MapPost("/", async ([FromRoute] Guid authorId, [FromBody] CreateEditPostRequest request, IAuthorRepository authorRepository, CancellationToken cancellationToken) =>
            {
                var author = await authorRepository.FindByIdAsync(authorId, cancellationToken);

                if (author is null)
                    return Results.NotFound();

                var post = author.CreatePost(request.Title, request.Content);

                await authorRepository.UpdateAsync(author, cancellationToken);

                return Results.Created();
            });

            group.MapGet("{id}", async ([FromRoute] Guid authorId, [FromRoute] Guid id, IAuthorRepository authorRepository, CancellationToken cancellationToken) =>
            {
                var author = await authorRepository.FindByIdAsync(authorId, cancellationToken);

                if (author is null)
                    return Results.NotFound();

                var post = author.Posts.FirstOrDefault(p => p.Id == id);

                return post is null
                    ? Results.NotFound()
                    : Results.Ok(post);
            });

            group.MapPut("{id}", async ([FromRoute] Guid authorId, [FromBody] CreateEditPostRequest request, [FromRoute] Guid id, IAuthorRepository authorRepository, CancellationToken cancellationToken) =>
            {
                var author = await authorRepository.FindByIdAsync(authorId, cancellationToken);

                if (author is null)
                    return Results.NotFound();

                var post = author.Posts.FirstOrDefault(p => p.Id == id);

                if (post is null)
                    return Results.NotFound();

                post.SetTitle(request.Title);
                post.SetContent(request.Content);

                await authorRepository.UpdateAsync(author, cancellationToken);

                return Results.Ok(post);
            });

            group.MapDelete("{id}", async ([FromRoute] Guid authorId, [FromRoute] Guid id, IAuthorRepository authorRepository, CancellationToken cancellationToken) =>
            {
                var author = await authorRepository.FindByIdAsync(authorId, cancellationToken);

                if (author is null)
                    return Results.NotFound();

                var removed = author.RemovePost(id);

                if (!removed)
                    return Results.NotFound();

                await authorRepository.UpdateAsync(author, cancellationToken);

                return Results.NoContent();
            });

            return builder;
        }
    }
}
