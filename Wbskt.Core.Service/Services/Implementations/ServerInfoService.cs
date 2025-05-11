using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Wbskt.Common.Contracts;
using Wbskt.Common.Providers;

namespace Wbskt.Core.Service.Services.Implementations;

public class ServerInfoService(ILogger<ServerInfoService> logger, IServerInfoProvider serverInfoProvider, IChannelsService channelsService, IAuthService authService) : IServerInfoService
{
    /// <summary>
    /// Map of each S.S with the list of channels assigned to it.
    /// </summary>
    private readonly IDictionary<int, HashSet<int>> serverChannelMap = new Dictionary<int, HashSet<int>>();
    private IDictionary<int, ServerInfo> allServers = new Dictionary<int, ServerInfo>();
    private readonly ILogger<ServerInfoService> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IServerInfoProvider serverInfoProvider = serverInfoProvider ?? throw new ArgumentNullException(nameof(serverInfoProvider));
    private readonly IChannelsService channelsService = channelsService ?? throw new ArgumentNullException(nameof(channelsService));
    private readonly IAuthService authService = authService ?? throw new ArgumentNullException(nameof(authService));

    public IReadOnlyCollection<ServerInfo> GetAll()
    {
        // gets all servers from DB
        var servers = serverInfoProvider.GetAllSocketServerInfo();
        var existingServersIds = allServers.Keys.ToList();
        allServers = servers.ToDictionary(s => s.ServerId, s => s);

        // new serverIds that came from the DB. (this is not cached yet)
        var newServerIds = allServers.Keys.Except(existingServersIds).ToList();

        foreach ( var serverId in newServerIds)
        {
            serverChannelMap.TryAdd(serverId, []);
        }

        var channels = channelsService.GetAll();

        foreach (var server in servers)
        {
            foreach (var channelId in channels.Where(ch => ch.ServerId == server.ServerId).Select(ch => ch.ChannelId))
            {
                serverChannelMap[server.ServerId].Add(channelId);
            }
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
        var serverId = serverChannelMap.Where(s => allServers[s.Key].Active).MinBy(kv => kv.Value.Count).Key;
        logger.LogDebug("available serverId: {serverId}", serverId);
        return serverId;
    }

    public async Task<bool> DispatchPayload(ClientPayload payload)
    {
        var publisherId = payload.PublisherId;

        var channels = channelsService.GetAll()
            .GroupBy(c => c.ChannelPublisherId)
            .ToDictionary(g => g.Key, g => g.Select(c => c.ChannelId).ToArray());

        var tasks = new List<Task>();

        if (channels.TryGetValue(publisherId, out var channelIds))
        {
            foreach (var serverChannel in serverChannelMap)
            {
                // Faster overlap check without creating a new collection
                // `channelIds` is the array of channels where payload needs to be dispatched
                // `serverChannel.Value` is the list of channels that the given S.S has
                // we just need to find if there is an overlap. if yes, dispatch
                if (serverChannel.Value.Any(channel => channelIds.Contains(channel)))
                {
                    logger.LogDebug("Dispatcher task queued for socket server: {serverId}, publisherId: {publisherId}", serverChannel.Key, payload);
                    tasks.Add(DispatchPayloadToServer(serverChannel.Key, publisherId, payload));
                }
            }
            await Task.WhenAll(tasks);
            return true;
        }

        return false;
    }

    public void UpdateServerStatus(int id, bool active)
    {
        allServers[id].Active = active;
        logger.LogDebug("updating status of server: {serverId} - {active}", id, active);
        serverInfoProvider.UpdateServerStatus(id, active);

        // the re-balance logic
        if (!active)
        {
            logger.LogInformation("re-balancing socket-server:channel (server-{serverId} is inactive)", id);
            // gets the channels of the inactive server
            var channelsIds = serverChannelMap[id];
            var updates = new List<(int, int)>();
            foreach (var channelId in channelsIds)
            {
                // distribute the channels of the offline server to other online server
                var activeServers = serverChannelMap.Where(s => allServers[s.Key].Active).ToList();
                if (activeServers.Count == 0)
                {
                    logger.LogWarning("failed re-balancing (no active socket-server)");
                    break;
                }

                var availableServerChannels = activeServers.MinBy(kv => kv.Value.Count);
                availableServerChannels.Value.Add(channelId);
                updates.Add((channelId, availableServerChannels.Key));
            }
            channelsService.UpdateServerIds(updates.ToArray());
        }
    }

    public void MapAllChannels()
    {
        GetAll();
        var channels = channelsService.GetAll();
        foreach (var server in allServers.Values)
        {
            var serverChannels = channels.Where(c => c.ServerId == server.ServerId);

            var channelIds = serverChannels.Select(channel => channel.ChannelId).ToHashSet();

            serverChannelMap.TryAdd(server.ServerId, channelIds);
        }
    }

    private async Task DispatchPayloadToServer(int serverKey, Guid publisherId, ClientPayload payload)
    {
        var token = authService.CreateCoreServerToken();
        var authHeader = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
        var server = allServers[serverKey];

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri($"http://{server.GetAddressWithFallback()}"),
            DefaultRequestHeaders = { Authorization = authHeader}
        };

        try
        {
            logger.LogDebug("post: {url}/dispatch/{publisher}", httpClient.BaseAddress, publisherId);
            var result = await httpClient.PostAsync($"dispatch/{publisherId}", JsonContent.Create(payload));
            if (!result.IsSuccessStatusCode)
            {
                logger.LogError("dispatch to {server} Id:({serverId}) failed with: {reason}", server.GetAddressWithFallback(), server.ServerId, result.ReasonPhrase);
            }
        }
        catch(Exception ex)
        {
            logger.LogWarning("error while request to {url}/dispatch/{publisher}, error: {details}", httpClient.BaseAddress, publisherId, ex.Message);
        }
    }
}
