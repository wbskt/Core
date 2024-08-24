using Wbskt.Core.Web.Database;

namespace Wbskt.Core.Web.Services.Implementations
{
    public class ChannelsService : IChannelsService
    {
        private readonly ILogger<ChannelsService> logger;
        private readonly IChannelsProvider channelsProvider;

        public ChannelsService(ILogger<ChannelsService> logger, IChannelsProvider channelsProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.channelsProvider = channelsProvider ?? throw new ArgumentNullException(nameof(channelsProvider));
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
            };
            
            details.ChannelId = channelsProvider.CreateChannel(details);
            return details;
        }

        public IEnumerable<ChannelDetails> GetChannelsForUser(int userId)
        {
            return channelsProvider.GetChannelsByUser(userId);
        }
    }
}
