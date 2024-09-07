namespace FaccToolkit.Persistence.EntityFramework.Abstractions.Tests
{
    public class TestModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string MyProperty { get; set; } = Guid.NewGuid().ToString();
    }
}
