using System.Collections.Generic;
using Wbskt.Common;
using Wbskt.Common.Contracts;
using Wbskt.Common.Exceptions;
using Wbskt.Common.Providers;

namespace Wbskt.Core.Service.Services.Implementations;

public class ChannelsService(ILogger<ChannelsService> logger, IChannelsProvider channelsProvider, Lazy<IServerInfoService> serverInfoService) : IChannelsService
{
    private readonly ILogger<ChannelsService> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IChannelsProvider channelsProvider = channelsProvider ?? throw new ArgumentNullException(nameof(channelsProvider));
    private readonly Lazy<IServerInfoService> serverInfoService = serverInfoService ?? throw new ArgumentNullException(nameof(serverInfoService));

    public ChannelDetails CreateChannel(ChannelRequest channel)
    {
        if (CheckIfUserHasSameChannelName(channel.UserId, channel.ChannelName))
        {
            throw WbsktExceptions.ChannelExists(channel.ChannelName);
        }

        var details = new ChannelDetails
        {
            UserId = channel.UserId,
            ChannelName = channel.ChannelName,
            RetentionTime = channel.RetentionTime,
            ChannelSecret = channel.ChannelSecret,
            ChannelPublisherId = channel.ChannelPublisherId == Guid.Empty ? Guid.NewGuid() : channel.ChannelPublisherId,
            ChannelSubscriberId = Guid.NewGuid(),
            // todo: this is only required for multi S.S setup, kept it since i guess the logic under the hood works
            ServerId = serverInfoService.Value.GetAvailableServerId()
        };

        details.ChannelId = channelsProvider.CreateChannel(details);
        return details;
    }

    public IReadOnlyCollection<ChannelDetails> GetAll()
    {
        return channelsProvider.GetAll();
    }

    public bool CheckIfUserHasSameChannelName(int userId, string channelName)
    {
        var channels = GetAll();
        return channels.Any(c => c.UserId == userId && c.ChannelName == channelName);
    }

    public IEnumerable<ChannelDetails> GetChannelsForUser(int userId)
    {
        return channelsProvider.GetChannelsByUser(userId);
    }

    public ChannelDetails GetChannelSubscriberId(Guid channelSubscriberId)
    {
        return channelsProvider.GetChannelBySubscriberId(channelSubscriberId);
    }

    public bool VerifyChannel(Guid channelSubscriberId, string channelSecret)
    {
        return GetChannelSubscriberId(channelSubscriberId).ChannelSecret.Equals(channelSecret);
    }

    public void UpdateServerIds((int Id, int ServerId)[] updates)
    {
        channelsProvider.UpdateServerIds(updates);
    }
}
