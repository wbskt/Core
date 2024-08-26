

namespace Wbskt.Core.Web.Services
{
    public interface IChannelsService
    {
        ChannelDetails CreateChannel(Channel channel);

        IEnumerable<ChannelDetails> GetChannelsForUser(int userId);
        ChannelDetails GetChannelSubscriberId(Guid channelSubscriberId);
    }
}
