using FaccToolkit.Domain.Abstractions;
using System;

namespace FaccToolkit.Examples.RichDomain.Aggregations.Authors
{
    public class Post : Entity<Guid>
    {
        public Title Title { get; private set; }

        public Content Content { get; private set; }

        public Post(Guid id, Title title, Content content) : base(id)
        {
            Title = title;
            Content = content;
        }

        public static Post Create(Title title, Content content)
        {
            if (title is null)
                throw new ArgumentNullException(nameof(title));

            if (content is null)
                throw new ArgumentNullException(nameof(content));

            return new Post(Guid.NewGuid(), title, content);
        }

        public void SetTitle(Title title)
        {
            if (title is null)
                throw new ArgumentNullException(nameof(title));

            Title = title;
        }

        public void SetContent(Content content)
        {
            if (content is null)
                throw new ArgumentNullException(nameof(content));

            Content = content;
        }
    }
}
