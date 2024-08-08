using System.Threading;
using System.Threading.Tasks;

namespace FaccToolkit.Caching.Abstractions
{
    public interface IModelSerializer
    {
        Task<string> SerializeAsync<TModel>(TModel value, CancellationToken cancellationToken)
            where TModel : class;

        Task<TModel> DeserializeAsync<TModel>(string value, CancellationToken cancellationToken)
            where TModel : class;
    }
}
