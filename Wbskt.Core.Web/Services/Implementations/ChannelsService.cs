using Wbskt.Core.Web.Database;

namespace Wbskt.Core.Web.Services.Implementations
{
    public class ChannelsService : IChannelsService
    {
        private readonly ILogger<ChannelsService> logger;
        private readonly IChannelsProvider channelsProvider;
        private readonly IServerHealthMonitor serverHealthMonitor;

        public ChannelsService(ILogger<ChannelsService> logger, IChannelsProvider channelsProvider, IServerHealthMonitor serverHealthMonitor)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.channelsProvider = channelsProvider ?? throw new ArgumentNullException(nameof(channelsProvider));
            this.serverHealthMonitor = serverHealthMonitor ?? throw new ArgumentNullException(nameof(serverHealthMonitor));
        }

        public ChannelDetails CreateChannel(Channel channel)
        {
            var details = new ChannelDetails
            {
                UserId = channel.UserId,
                ChannelName = channel.ChannelName,
                RetentionTime = channel.RetentionTime,
                ChannelPublisherId = Guid.NewGuid(),
                ChannelSubscriberId = Guid.NewGuid(),
                ServerId = serverHealthMonitor.GetAvailableServerId()
            };
            
            details.ChannelId = channelsProvider.CreateChannel(details);
            return details;
        }

        public IEnumerable<ChannelDetails> GetChannelsForUser(int userId)
        {
            return channelsProvider.GetChannelsByUser(userId);
        }

        public ChannelDetails GetChannelSubscriberId(Guid channelSubscriberId)
        {
            return channelsProvider.GetChannelSubscriberId(channelSubscriberId);
        }
    }
}
