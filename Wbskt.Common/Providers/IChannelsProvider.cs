using Wbskt.Common.Contracts;

namespace Wbskt.Common.Providers;

public interface IChannelsProvider
{
    int CreateChannel(ChannelDetails channel);
    IReadOnlyCollection<ChannelDetails> GetAll();
    IReadOnlyCollection<ChannelDetails> GetChannelsByUser(int userId);

    ChannelDetails GetChannelSubscriberId(Guid channelSubscriberId);
}
