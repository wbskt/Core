
using Wbskt.Core.Web.Database;

namespace Wbskt.Core.Web.Services.Implementations
{
    // todo logical problems.. imagine ss registering in between
    public class ServerInfoService : IServerInfoService
    {
        private IDictionary<int, ServerInfo> allServers;
        private readonly ILogger<ServerInfoService> logger;
        private readonly IServerInfoProvider serverInfoProvider;
        private readonly IChannelsService channelsService;
        private readonly IDictionary<int, Stack<int>> serverChannelMap;

        public ServerInfoService(ILogger<ServerInfoService> logger, IServerInfoProvider serverInfoProvider, IChannelsService channelsService)
        {
            allServers = new Dictionary<int, ServerInfo>();
            serverChannelMap = new Dictionary<int, Stack<int>>();
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.serverInfoProvider = serverInfoProvider ?? throw new ArgumentNullException(nameof(serverInfoProvider));
            this.channelsService = channelsService ?? throw new ArgumentNullException(nameof(channelsService));
            MapAllChannels();
        }
        
        public IReadOnlyCollection<ServerInfo> GetAll()
        {
            var servers = serverInfoProvider.GetAll();
            var existingServersIds = allServers.Keys.ToList();
            allServers = servers.ToDictionary(s => s.ServerId, s => s);
            var newServerIds = allServers.Keys.Except(existingServersIds).ToList();

            foreach ( var serverIds in newServerIds)
            {
                serverChannelMap.TryAdd(serverIds, new Stack<int>());
            }

            return servers;
        }

        public ServerInfo GetServerById(int id)
        {
            return allServers[id];
        }

        public int GetAvailableServerId()
        {
            return serverChannelMap.MinBy(kv => kv.Value.Count).Key;
        }

        public void UpdateServerStatus(int id, bool active)
        {
            allServers[id].Active = active;
            serverInfoProvider.UpdateServerStatus(id, active);

            if (active)
            {
                // the re-balance logic
                var channelsIds = serverChannelMap[id];
                foreach (var channelId in channelsIds)
                {
                    var stack = serverChannelMap.MinBy(kv => kv.Value.Count).Value;
                    stack.Push(channelId);
                }
            }
        }

        private void MapAllChannels()
        {
            GetAll();
            var channels = channelsService.GetAll();
            foreach (var server in allServers.Values)
            {
                var serverChannels = channels.Where(c => c.ServerId == server.ServerId);
                //if (!serverChannels.Any())
                //{
                //    continue;
                //}

                var stack = new Stack<int>();
                foreach (var channel in serverChannels)
                {
                    stack.Push(channel.ChannelId);
                }

                serverChannelMap.TryAdd(server.ServerId, stack);
            }
        }
    }
}
