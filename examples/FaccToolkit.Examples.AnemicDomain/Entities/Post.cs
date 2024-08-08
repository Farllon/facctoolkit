using FaccToolkit.Domain.Abstractions;
using System;

namespace FaccToolkit.Examples.AnemicDomain.Entities
{
    public class Post : Entity<Guid>, IAuditable
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Guid AuthorId { get; set; }

        public Post(Guid id,string title, string content, Guid authorId) : base(id)
        {
            Title = title;
            Content = content;
            CreatedAt = DateTime.UtcNow;
            AuthorId = authorId;
        }

        public Post(string title, string content, Guid authorId) : this(Guid.NewGuid(), title, content, authorId)
        {

        }
    }
}
