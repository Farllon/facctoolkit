namespace FaccToolkit.Persistence.MongoDb.Abstractions.Tests
{
    public class TestModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string MyProperty { get; set; } = Guid.NewGuid().ToString();
    }
}
