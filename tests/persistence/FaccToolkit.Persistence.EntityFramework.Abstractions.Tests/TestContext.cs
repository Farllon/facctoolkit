using Microsoft.EntityFrameworkCore;

namespace FaccToolkit.Persistence.EntityFramework.Abstractions.Tests
{
    public class TestContext : DbContext
    {
        public DbSet<TestModel> Tests { get; set; }

        public TestContext(DbContextOptions options) : base(options)
        {
            
        }

        public TestContext()
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TestModel>().HasKey(e => e.Id);
        }
    }
}
