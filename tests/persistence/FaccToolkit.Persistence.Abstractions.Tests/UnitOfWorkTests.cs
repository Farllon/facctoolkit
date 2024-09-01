using FaccToolkit.Domain.Abstractions;
using Moq;

namespace FaccToolkit.Persistence.Abstractions
{
    public class UnitOfWorkTests
    {
        private readonly Mock<IServiceProvider> _serviceProviderMock;

        public UnitOfWorkTests()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
        }

        private class TestUnitOfWork : UnitOfWork
        {
            public TestUnitOfWork(IServiceProvider provider) : base(provider)
            {
            }

            public override Task<bool> IsInTransactionAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(true);
            }

            public override Task BeginTransactionAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public override Task CommitTransactionAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public override Task AbortTransactionAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public override void Dispose()
            {

            }
        }

        [Fact]
        public void GetRepository_ShouldReturnRepositoryInstance()
        {
            var repositoryMock = new Mock<IRepository<IEntity<int>, int>>();
            _serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IRepository<IEntity<int>, int>)))
                .Returns(repositoryMock.Object);

            var unitOfWork = new TestUnitOfWork(_serviceProviderMock.Object);

            var repository = unitOfWork.GetRepository<IRepository<IEntity<int>, int>, IEntity<int>, int>();

            Assert.NotNull(repository);
            Assert.IsAssignableFrom<IRepository<IEntity<int>, int>>(repository);
        }

        [Fact]
        public async Task IsInTransactionAsync_ShouldReturnTrue()
        {            
            var unitOfWork = new TestUnitOfWork(_serviceProviderMock.Object);

            var result = await unitOfWork.IsInTransactionAsync(CancellationToken.None);

            Assert.True(result);
        }

        [Fact]
        public async Task BeginTransactionAsync_ShouldCompleteTask()
        {
            var unitOfWork = new TestUnitOfWork(_serviceProviderMock.Object);
            
            await unitOfWork.BeginTransactionAsync(CancellationToken.None);
        }

        [Fact]
        public async Task CommitTransactionAsync_ShouldCompleteTask()
        {            
            var unitOfWork = new TestUnitOfWork(_serviceProviderMock.Object);
   
            await unitOfWork.CommitTransactionAsync(CancellationToken.None);
        }

        [Fact]
        public async Task AbortTransactionAsync_ShouldCompleteTask()
        {            
            var unitOfWork = new TestUnitOfWork(_serviceProviderMock.Object);
   
            await unitOfWork.AbortTransactionAsync(CancellationToken.None);
        }

        [Fact]
        public void Dispose_ShouldDisposeUnitOfWork()
        {            
            var unitOfWork = new TestUnitOfWork(_serviceProviderMock.Object);

            unitOfWork.Dispose();
        }
    }
}