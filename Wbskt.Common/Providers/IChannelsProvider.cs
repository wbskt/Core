using Wbskt.Common.Contracts;

namespace Wbskt.Common.Providers;

public interface IChannelsProvider
{
    int CreateChannel(ChannelDetails channel);
    void UpdateServerId(int channelId, int serverId);
    IReadOnlyCollection<ChannelDetails> GetAll();
    IReadOnlyCollection<ChannelDetails> GetChannelsByUser(int userId);

    ChannelDetails GetChannelSubscriberId(Guid channelSubscriberId);
    IReadOnlyCollection<ChannelDetails> GetChannelPublisherId(Guid channelPublisherId);
}
