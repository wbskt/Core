using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using Wbskt.Common.Contracts;
using Wbskt.Common.Providers.Implementations;

namespace Wbskt.Common.Providers.Cache
{
    internal sealed class CachedChannelsProvider : IChannelsProvider
    {
        private static readonly string ServerType = Environment.GetEnvironmentVariable(nameof(ServerType)) ?? Constants.ServerType.CoreServer;

        private readonly List<ChannelDetails> channels = [];
        private readonly object @lock = new();
        private readonly ILogger<CachedChannelsProvider> logger;
        private readonly ChannelsProvider channelsProvider;

        public CachedChannelsProvider(ILogger<CachedChannelsProvider> logger, ChannelsProvider channelsProvider)
        {
            this.logger = logger;
            this.channelsProvider = channelsProvider;

            channelsProvider.RegisterSqlDependency(OnDatabaseChange);
        }

        public int CreateChannel(ChannelDetails channel)
        {
            if (ServerType != Constants.ServerType.CoreServer)
            {
                logger.LogError("only core server perform this operation: {operationName}", nameof(CreateChannel));
                return -1;
            }

            var id = channelsProvider.CreateChannel(channel);
            if (id <= 0)
            {
                return id;
            }

            lock (@lock)
            {
                channels.Add(channel);
            }
            return id;
        }

        public IReadOnlyCollection<ChannelDetails> GetAll()
        {
            if (ServerType != Constants.ServerType.CoreServer)
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

        public IReadOnlyCollection<ChannelDetails> GetChannelByPublisherId(Guid channelPublisherId)
        {
            return [.. channels.Where(c => c.ChannelPublisherId == channelPublisherId)];
        }

        public IReadOnlyCollection<ChannelDetails> GetChannelsByUser(int userId)
        {
            return [.. channels.Where(c => c.UserId == userId)];
        }

        public ChannelDetails GetChannelBySubscriberId(Guid channelSubscriberId)
        {
            return channels.FirstOrDefault(c => c.ChannelSubscriberId == channelSubscriberId) ?? throw new InvalidOperationException();
        }

        public void UpdateServerIds((int Id, int ServerId)[] updates)
        {
            if (ServerType != Constants.ServerType.CoreServer)
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

            channelsProvider.UpdateServerIds(updates);
        }

        private void RefreshCache()
        {
            var records = channelsProvider.GetAll();
            lock (@lock)
            {
                channels.Clear();
                channels.AddRange(records);
            }
        }

        private void OnDatabaseChange(object sender, SqlNotificationEventArgs e)
        {
            logger.LogInformation("database change detected: {Info}", e.Info);

            RefreshCache();
            channelsProvider.RegisterSqlDependency(OnDatabaseChange); // re-register after change
        }
    }
}
