using FaccToolkit.Domain.Abstractions;

namespace FaccToolkit.Persistence.Extensions.Caching.AnemicDomain.Tests
{
    public class TestEntity : Entity<Guid>
    {
        public TestEntity() : base(Guid.NewGuid())
        {
        }
    }
}
