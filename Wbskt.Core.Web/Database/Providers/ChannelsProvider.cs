namespace Wbskt.Core.Web.Database.Providers
{
    public class ChannelsProvider : IChannelsProvider
    {
        private readonly ILogger<ChannelsProvider> logger;
        private readonly IConnectionStringProvider connectionStringProvider;

        public ChannelsProvider(ILogger<ChannelsProvider> logger, IConnectionStringProvider connectionStringProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.connectionStringProvider = connectionStringProvider ?? throw new ArgumentNullException(nameof(connectionStringProvider));
        }
    }
}
