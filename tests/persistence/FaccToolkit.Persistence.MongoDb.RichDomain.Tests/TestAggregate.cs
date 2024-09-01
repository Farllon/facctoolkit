using FaccToolkit.Domain.Rich;

namespace FaccToolkit.Persistence.MongoDb.RichDomain.Tests
{
    public class TestAggregate : AggregateRoot<Guid>
    {
        public TestAggregate() : base(Guid.NewGuid())
        {
        }

        public void Created()
        {
            EnqueueEvent(new CreatedEvent(Id));
        }

        public void Updated()
        {
            EnqueueEvent(new UpdatedEvent(Id));
        }

        public void Deleted()
        {
            EnqueueEvent(new DeletedEvent(Id));
        }

        public record CreatedEvent(Guid Id) : IDomainEvent<Guid>;
        public record UpdatedEvent(Guid Id) : IDomainEvent<Guid>;
        public record DeletedEvent(Guid Id) : IDomainEvent<Guid>;
    }
}