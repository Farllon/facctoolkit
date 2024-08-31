
using DotNet.Testcontainers.Builders;
using Testcontainers.MongoDb;

namespace FaccToolkit.Persistence.MongoDb.Abstractions.Tests
{
    public class MongoDbFixture : IAsyncLifetime
    {
        private readonly MongoDbContainer _container;

        public string ConnectionString => _container.GetConnectionString();
        public string ContainerId => _container.Id;

        public MongoDbFixture()
        {
            _container = new MongoDbBuilder()
                .WithUsername(string.Empty)
                .WithPassword(string.Empty)
                .WithImage("mongo:latest")
                .WithExtraHost("host.docker.internal", "host-gateway")
                .WithCommand("--replSet", "rs0")
                .WithWaitStrategy(Wait.ForUnixContainer())
                .Build();
        }

        public virtual async Task InitializeAsync()
        {
            await _container.StartAsync();
            await _container.ExecScriptAsync($@"rs.initiate({{
                _id: 'rs0',
                members: [
                    {{
                        _id: 0,
                        host: 'host.docker.internal:{_container.GetMappedPublicPort(27017)}'
                    }}
                ]
            }})");
        }

        public virtual Task DisposeAsync()
        {
            return _container
                .DisposeAsync()
                .AsTask();
        }
    }
}
