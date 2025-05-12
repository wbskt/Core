using Microsoft.Extensions.Logging;
using Wbskt.Common.Contracts;
using Wbskt.Common.Exceptions;
using Wbskt.Common.Providers.Implementations;

namespace Wbskt.Common.Providers.Cache
{
    internal sealed class CachedChannelsProvider(ILogger<CachedChannelsProvider> logger, ChannelsProvider channelsProvider) : IChannelsProvider
    {
        private static readonly string ServerType = Environment.GetEnvironmentVariable(nameof(ServerType)) ?? Constants.ServerType.CoreServer.ToString();

        private readonly List<ChannelDetails> channels = [];
        private readonly object @lock = new();

        // channelsProvider.RegisterSqlDependency(OnDatabaseChange);

        public IReadOnlyCollection<ChannelDetails> GetAllByChannelPublisherId(Guid channelPublisherId)
        {
            return [.. channels.Where(c => c.ChannelPublisherId == channelPublisherId)];
        }

        public IReadOnlyCollection<ChannelDetails> GetAllByChannelServerId(int serverId)
        {
            return [.. channels.Where(c => c.ServerId == serverId)];
        }

        public IReadOnlyCollection<ChannelDetails> GetAllByChannelUserId(int userId)
        {
            return [.. channels.Where(c => c.UserId == userId)];
        }

        public IReadOnlyCollection<ChannelDetails> GetAll()
        {
            if (ServerType != Constants.ServerType.CoreServer.ToString())
            {
                logger.LogError("only core server perform this operation: {operationName}", nameof(GetAll));
                return [];
            }

            lock (@lock)
            {
                if (channels.Count != 0)
                {
                    return [.. channels]; // return a copy to prevent external mutation
                }

                var records = channelsProvider.GetAll();
                channels.AddRange(records);

                return [.. channels]; // return a copy to prevent external mutation
            }
        }

        public ChannelDetails GetByChannelSubscriberId(Guid channelSubscriberId)
        {
            return channels.FirstOrDefault(c => c.ChannelSubscriberId == channelSubscriberId) ?? throw WbsktExceptions.ChannelSubscriberIdNotExists(channelSubscriberId);
        }

        public ChannelDetails GetByChannelId(int channelId)
        {
            return channels.FirstOrDefault(c => c.ChannelId == channelId) ?? throw WbsktExceptions.ChannelIdNotExists(channelId);
        }

        public int CreateChannel(ChannelDetails channel)
        {
            if (ServerType != Constants.ServerType.CoreServer.ToString())
            {
                logger.LogError("only core server perform this operation: {operationName}", nameof(CreateChannel));
                return -1;
            }

            var id = channelsProvider.CreateChannel(channel);
            if (id <= 0)
            {
                throw new InvalidOperationException("this shouldn't happen. DB returned with id <= 0. (seek help)");
            }

            lock (@lock)
            {
                channels.Add(channel);
            }
            return id;
        }

        public void UpdateServerIds((int Id, int ServerId)[] updates)
        {
            if (ServerType != Constants.ServerType.CoreServer.ToString())
            {
                logger.LogError("only core server perform this operation: {operationName}", nameof(UpdateServerIds));
                return;
            }

            var dict = updates.ToDictionary(u => u.Id, u => u.ServerId);
            foreach (var channel in channels)
            {
                if (dict.TryGetValue(channel.ChannelId, out var value))
                {
                    channel.ServerId = value;
                }
            }

            // channelsProvider.UpdateServerIds(updates);
        }

        // private void RefreshCache()
        // {
        //     var records = channelsProvider.GetAll();
        //     lock (@lock)
        //     {
        //         channels.Clear();
        //         channels.AddRange(records);
        //     }
        // }
        //
        // private void OnDatabaseChange(object sender, SqlNotificationEventArgs e)
        // {
        //     logger.LogInformation("database change detected: {Info}", e.Info);
        //
        //     RefreshCache();
        //     channelsProvider.RegisterSqlDependency(OnDatabaseChange); // re-register after change
        // }
    }
}
