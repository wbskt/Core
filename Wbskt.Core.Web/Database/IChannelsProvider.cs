using Wbskt.Core.Web.Services;

namespace Wbskt.Core.Web.Database
{
    public interface IChannelsProvider
    {
        int CreateChannel(ChannelDetails channel);
        IReadOnlyCollection<ChannelDetails> GetAll();
        IReadOnlyCollection<ChannelDetails> GetChannelsByUser(int userId);

        ChannelDetails GetChannelSubscriberId(Guid channelSubscriberId);
    }
}
