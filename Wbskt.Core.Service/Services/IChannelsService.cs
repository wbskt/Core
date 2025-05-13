

using Wbskt.Common.Contracts;

namespace Wbskt.Core.Service.Services;

public interface IChannelsService
{
    ChannelDetails CreateChannel(ChannelCreationRequest channelCreation);

    IReadOnlyCollection<ChannelDetails> GetAll();

    IEnumerable<ChannelDetails> GetChannelsForUser(int userId);

    ChannelDetails GetChannelSubscriberId(Guid channelSubscriberId);

    bool VerifyChannel(ClientChannel[] channels);
}
