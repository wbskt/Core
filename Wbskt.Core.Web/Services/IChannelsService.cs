

namespace Wbskt.Core.Web.Services
{
    public interface IChannelsService
    {
        ChannelDetails CreateChannel(Channel channel);
        IReadOnlyCollection<ChannelDetails> GetAll();
        IEnumerable<ChannelDetails> GetChannelsForUser(int userId);
        ChannelDetails GetChannelSubscriberId(Guid channelSubscriberId);
    }
}
