using FaccToolkit.Domain.Abstractions;

namespace FaccToolkit.Persistence.MongoDb.AnemicDomain.Tests
{
    public class TestEntity : Entity<Guid>
    {
        public TestEntity() : base(Guid.NewGuid())
        {
        }
    }
}