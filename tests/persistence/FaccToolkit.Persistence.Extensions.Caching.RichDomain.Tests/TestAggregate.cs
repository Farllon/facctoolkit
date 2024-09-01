using FaccToolkit.Domain.Rich;

namespace FaccToolkit.Persistence.Extensions.Caching.RichDomain.Tests
{
    public class TestAggregate : AggregateRoot<Guid>
    {
        public TestAggregate() : base(Guid.NewGuid())
        {
        }
    }
}
