using Microsoft.Extensions.DependencyInjection;
using Wbskt.Common.Providers;
using Wbskt.Common.Providers.Implementations;

namespace Wbskt.Common;

public static class DependencyInjection
{
    public static void ConfigureCommonServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IUsersProvider, UsersProvider>();
        serviceCollection.AddSingleton<IClientProvider, ClientProvider>();
        serviceCollection.AddSingleton<IChannelsProvider, ChannelsProvider>();
        serviceCollection.AddSingleton<IServerInfoProvider, ServerInfoProvider>();
        serviceCollection.AddSingleton<IConnectionStringProvider, ConnectionStringProvider>();
    }
}
