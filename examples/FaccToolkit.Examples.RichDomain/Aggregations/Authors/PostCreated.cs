using FaccToolkit.Domain.Rich;
using MediatR;
using System;

namespace FaccToolkit.Examples.RichDomain.Aggregations.Authors
{
    public class PostCreated : IDomainEvent<Guid>, INotification
    {
        public Guid Id { get; }

        public Guid PostId { get; }

        public string Title { get; }

        public string Content { get; }

        public PostCreated(Guid postId, string title, string content)
        {
            Id = Guid.NewGuid();
            PostId = postId;
            Title = title;
            Content = content;
        }
    }
}
