
using Wbskt.Common;
using Wbskt.Common.Contracts;
using Wbskt.Common.Providers;

namespace Wbskt.Core.Web.Services.Implementations;

// todo logical problems.. imagine ss registering in between
public class ServerInfoService : IServerInfoService
{
    private IDictionary<int, ServerInfo> allServers;
    private readonly ILogger<ServerInfoService> logger;
    private readonly IServerInfoProvider serverInfoProvider;
    private readonly IChannelsService channelsService;

    /// <summary>
    /// Map of each S.S with the list of channels assigned to it.
    /// </summary>
    private readonly IDictionary<int, List<int>> serverChannelMap;

    public ServerInfoService(ILogger<ServerInfoService> logger, IServerInfoProvider serverInfoProvider, IChannelsService channelsService)
    {
        allServers = new Dictionary<int, ServerInfo>();
        serverChannelMap = new Dictionary<int, List<int>>();
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.serverInfoProvider = serverInfoProvider ?? throw new ArgumentNullException(nameof(serverInfoProvider));
        this.channelsService = channelsService ?? throw new ArgumentNullException(nameof(channelsService));
        MapAllChannels();
    }

    public IReadOnlyCollection<ServerInfo> GetAll()
    {
        // gets all servers from DB
        var servers = serverInfoProvider.GetAll();
        var existingServersIds = allServers.Keys.ToList();
        allServers = servers.ToDictionary(s => s.ServerId, s => s);

        // new serverIds that came from the DB. (this is not cached yet)
        var newServerIds = allServers.Keys.Except(existingServersIds).ToList();

        foreach ( var serverIds in newServerIds)
        {
            serverChannelMap.TryAdd(serverIds, new List<int>());
        }

        return servers;
    }

    public ServerInfo GetServerById(int id)
    {
        return allServers[id];
    }

    public int GetAvailableServerId()
    {
        // gets the S.S with the least amount of channels. (for a rudimentary load balancing)
        return serverChannelMap.Where(s => allServers[s.Key].Active).MinBy(kv => kv.Value.Count).Key;
    }

    public void UpdateServerStatus(int id, bool active)
    {
        allServers[id].Active = active;
        serverInfoProvider.UpdateServerStatus(id, active);

        // the re-balance logic
        if (!active)
        {
            // gets the channels of the inactive server
            var channelsIds = serverChannelMap[id];
            foreach (var channelId in channelsIds)
            {
                // distribute the channels of the offline server to other online server
                var channelIds = serverChannelMap.Where(s => allServers[s.Key].Active).MinBy(kv => kv.Value.Count).Value;
                channelIds.Add(channelId);
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

            var channelIds = serverChannels.Select(channel => channel.ChannelId).ToList();

            serverChannelMap.TryAdd(server.ServerId, channelIds);
        }
    }
}
