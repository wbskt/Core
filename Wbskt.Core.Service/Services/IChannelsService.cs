

using Wbskt.Common.Contracts;

namespace Wbskt.Core.Service.Services;

public interface IChannelsService
{
    ChannelDetails CreateChannel(ChannelRequest channel);

    IReadOnlyCollection<ChannelDetails> GetAll();

    IEnumerable<ChannelDetails> GetChannelsForUser(int userId);

    ChannelDetails GetChannelSubscriberId(Guid channelSubscriberId);

    bool VerifyChannel(Guid requestChannelSubscriberId, string requestChannelSecret);

    void UpdateServerIds((int Id, int ServerId)[] updates);
}
