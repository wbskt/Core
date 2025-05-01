using Wbskt.Common.Contracts;

namespace Wbskt.Common.Providers;

public interface IChannelsProvider
{
    int CreateChannel(ChannelDetails channel);

    void UpdateServerIds((int Id, int ServerId)[] updates);

    IReadOnlyCollection<ChannelDetails> GetAll();

    IReadOnlyCollection<ChannelDetails> GetChannelsByUser(int userId);

    ChannelDetails GetChannelBySubscriberId(Guid channelSubscriberId);

    IReadOnlyCollection<ChannelDetails> GetChannelByPublisherId(Guid channelPublisherId);
}
