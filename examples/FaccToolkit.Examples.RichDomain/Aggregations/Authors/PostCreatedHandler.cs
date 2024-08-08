using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Examples.RichDomain.Aggregations.Authors
{
    public class PostCreatedHandler : INotificationHandler<PostCreated>
    {
        public Task Handle(PostCreated notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[{DateTime.UtcNow}] {notification.Id} {notification.Title}");

            return Task.CompletedTask;
        }
    }
}
