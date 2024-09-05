using FaccToolkit.Examples.AnemicDomain.Entities;
using FaccToolkit.Examples.AnemicDomain.Repositories;
using FaccToolkit.Examples.MongoDb.AnemicDomainApi.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FaccToolkit.Examples.MongoDb.AnemicDomainApi.Endpoints
{
    public static class PostEndpoint
    {
        public static IEndpointRouteBuilder MapPost(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("{authorId}/posts");

            group.MapGet("/", async ([FromRoute] Guid authorId, IPostRepository postRepository, CancellationToken cancellationToken) =>
            {
                var posts = await postRepository.GetByAuthorIdAsync(authorId, cancellationToken);

                return posts.Count > 0
                ? Results.Ok(posts)
                : Results.NoContent();
            });

            group.MapPost("/", async ([FromRoute] Guid authorId, [FromBody] CreateEditPostRequest request, IPostRepository postRepository, CancellationToken cancelationToken) =>
            {
                var post = new Post(request.Title, request.Content, authorId);

                await postRepository.InsertAsync(post, cancelationToken);

                return Results.Created();
            });

            group.MapGet("{id}", async ([FromRoute] Guid authorId, [FromRoute] Guid id, IPostRepository postRepository, CancellationToken cancellationToken) =>
            {
                var post = await postRepository.FindByIdAsync(id, cancellationToken);

                return post is null
                    ? Results.NotFound()
                    : Results.Ok(post);
            });

            group.MapPut("{id}", async ([FromRoute] Guid authorId, [FromBody] CreateEditPostRequest request, [FromRoute] Guid id, IPostRepository postRepository, CancellationToken cancellationToken) =>
            {
                var post = new Post(authorId, request.Title, request.Content, authorId);

                await postRepository.UpdateAsync(post, cancellationToken);

                return Results.Ok(post);
            });

            group.MapDelete("{id}", async ([FromRoute] Guid authorId, [FromRoute] Guid id, IPostRepository postRepository, CancellationToken cancellationToken) =>
            {
                await postRepository.DeleteAsync(id, cancellationToken);

                return Results.NoContent();
            });

            return builder;
        }
    }
}
