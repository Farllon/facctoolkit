using FaccToolkit.Caching.Abstractions;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Caching.Serializers.System.Text.Json
{
    public class ModelSerializer : IModelSerializer
    {
        protected readonly JsonSerializerOptions _options;

        public ModelSerializer(IOptions<JsonSerializerOptions> options)
        {
            _options = options.Value ?? JsonSerializerOptions.Default;
        }

        public Task<TModel> DeserializeAsync<TModel>(string value, CancellationToken cancellationToken) where TModel : class
            => Task.FromResult(JsonSerializer.Deserialize<TModel>(value, _options)!);

        public Task<string> SerializeAsync<TModel>(TModel value, CancellationToken cancellationToken) where TModel : class
            => Task.FromResult(JsonSerializer.Serialize(value, _options));
    }
}
