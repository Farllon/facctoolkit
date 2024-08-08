using System.Threading.Tasks;
using System.Threading;

namespace FaccToolkit.Caching.Abstractions
{
    public interface ICacheFacade
    {
        Task<TModel?> TryGetAsync<TModel>(string key, CancellationToken cancellationToken)
            where TModel : class;

        Task SetAsync<TModel>(string key, TModel value, CancellationToken cancellationToken)
            where TModel : class;

        Task ExpiryAsync(string key, CancellationToken cancellationToken);

        string GenerateKey<TModel>(string suffix)
            where TModel : class;
    }
}
