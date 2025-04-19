
using Microsoft.Extensions.Configuration;

namespace Wbskt.Common.Providers.Implementations;

internal sealed class ConnectionStringProvider : IConnectionStringProvider
{
    public ConnectionStringProvider(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        ConnectionString = configuration["ConnectionStrings:Database"]!;
    }

    public string ConnectionString { get; }
}
