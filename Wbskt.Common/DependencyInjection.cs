using Microsoft.Extensions.DependencyInjection;
using Wbskt.Common.Providers;
using Wbskt.Common.Providers.Cache;
using Wbskt.Common.Providers.Implementations;

namespace Wbskt.Common;

public static class DependencyInjection
{
    public static void ConfigureCommonServices(this IServiceCollection serviceCollection)
    {
        // serviceCollection.AddSingleton<ClientProvider>();
        serviceCollection.AddSingleton<ChannelsProvider>();
        serviceCollection.AddSingleton<ServerInfoProvider>();
        serviceCollection.AddSingleton<IUsersProvider, UsersProvider>();
        serviceCollection.AddSingleton<IClientProvider, ClientProvider>();
        serviceCollection.AddSingleton<IChannelsProvider, CachedChannelsProvider>();
        serviceCollection.AddSingleton<IServerInfoProvider, CachedServerInfoProvider>();
        serviceCollection.AddSingleton<IConnectionStringProvider, ConnectionStringProvider>();
    }
}
