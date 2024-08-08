using FaccToolkit.Domain.Rich;
using System;
using System.Collections.Generic;

namespace FaccToolkit.Examples.RichDomain.Aggregations.Authors
{
    public class Author : AggregateRoot<Guid>
    {
        public Name Name { get; private set; }

        private readonly List<Post> _posts;
        public IReadOnlyCollection<Post> Posts => _posts;

        public Author(Guid id, Name name, List<Post> posts) : base(id)
        {
            _posts = posts;

            Name = name;
        }

        public static Author Create(Name name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return new Author(Guid.NewGuid(), name, new List<Post>());
        }

        public void SetName(Name name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        public Post CreatePost(Title title, Content content)
        {
            var post = Post.Create(title, content);

            _posts.Add(post);

            EnqueueEvent(new PostCreated(
                post.Id,
                post.Title.Value,
                post.Content.Value));

            return post;
        }

        public bool RemovePost(Guid postId)
        {
            var removed = _posts.RemoveAll(p => p.Id == postId);

            return removed == 1;
        }
    }
}
