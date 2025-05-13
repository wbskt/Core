using Wbskt.Common.Contracts;
using Wbskt.Common.Exceptions;
using Wbskt.Common.Providers;

namespace Wbskt.Core.Service.Services.Implementations;

public class ChannelsService(ILogger<ChannelsService> logger, IChannelsProvider channelsProvider) : IChannelsService
{
    private readonly ILogger<ChannelsService> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IChannelsProvider channelsProvider = channelsProvider ?? throw new ArgumentNullException(nameof(channelsProvider));

    public ChannelDetails CreateChannel(ChannelCreationRequest channelCreation)
    {
        if (CheckIfUserHasSameChannelName(channelCreation.UserId, channelCreation.ChannelName))
        {
            throw WbsktExceptions.ChannelExists(channelCreation.ChannelName);
        }

        var details = new ChannelDetails
        {
            UserId = channelCreation.UserId,
            ChannelName = channelCreation.ChannelName,
            ChannelSecret = channelCreation.ChannelSecret,
            ChannelPublisherId = channelCreation.ChannelPublisherId == Guid.Empty ? Guid.NewGuid() : channelCreation.ChannelPublisherId,
            ChannelSubscriberId = Guid.NewGuid(),
        };

        details.ChannelId = channelsProvider.CreateChannel(details);
        return details;
    }

    public IReadOnlyCollection<ChannelDetails> GetAll()
    {
        return channelsProvider.GetAll();
    }

    public IEnumerable<ChannelDetails> GetChannelsForUser(int userId)
    {
        return channelsProvider.GetAllByChannelUserId(userId);
    }

    public ChannelDetails GetChannelSubscriberId(Guid channelSubscriberId)
    {
        return channelsProvider.GetByChannelSubscriberId(channelSubscriberId)!;
    }

    public bool VerifyChannel(ClientChannel[] channels)
    {
        foreach (var channel in channels)
        {
            if (GetChannelSubscriberId(channel.ChannelSubscriberId).ChannelSecret.Equals(channel.ChannelSecret))
            {
                continue;
            }

            logger.LogWarning("channel subscription id: '{subId}' does not match the secret", channel.ChannelSubscriberId);
            return false;
        }
        return true;
    }

    private bool CheckIfUserHasSameChannelName(int userId, string channelName)
    {
        var channels = GetAll();
        return channels.Any(c => c.UserId == userId && c.ChannelName == channelName);
    }
}
