
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Wbskt.Common;
using Wbskt.Common.Contracts;
using Wbskt.Common.Providers;

namespace Wbskt.Core.Web.Services.Implementations;

// todo logical problems.. imagine ss registering in between
public class ServerInfoService : IServerInfoService
{

    /// <summary>
    /// Map of each S.S with the list of channels assigned to it.
    /// </summary>
    private readonly IDictionary<int, List<int>> serverChannelMap;
    private IDictionary<int, ServerInfo> allServers;
    private readonly ILogger<ServerInfoService> logger;
    private readonly IServerInfoProvider serverInfoProvider;
    private readonly IChannelsService channelsService;
    private readonly IAuthService authService;

    public ServerInfoService(ILogger<ServerInfoService> logger, IServerInfoProvider serverInfoProvider, IChannelsService channelsService, IAuthService authService)
    {
        this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
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

        foreach ( var serverId in newServerIds)
        {
            serverChannelMap.TryAdd(serverId, new List<int>());
        }

        var channels = channelsService.GetAll();

        foreach (var server in servers)
        {
            serverChannelMap[server.ServerId].AddRange(channels.Where(ch => ch.ServerId == server.ServerId).Select(ch => ch.ChannelId));
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

    public async Task DispatchPayload(Guid publisherId, ClientPayload payload)
    {
        var channels = channelsService.GetAll().ToDictionary(c => c.ChannelPublisherId, c => c.ChannelId);
        var tasks = new List<Task>();
        foreach (var serverChannel in serverChannelMap)
        {
            if (serverChannel.Value.Contains(channels[publisherId]))
            {
                tasks.Add(DispatchPayloadToServer(serverChannel.Key, publisherId, payload));
            }
        }

        await Task.WhenAll(tasks);
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
                var activeServers = serverChannelMap.Where(s => allServers[s.Key].Active).ToList();
                if (activeServers.Count == 0)
                {
                    continue;
                }

                var availableServerChannel = activeServers.MinBy(kv => kv.Value.Count);
                channelsService.UpdateServerId(channelId, availableServerChannel.Key);
                availableServerChannel.Value.Add(channelId);
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

    private async Task DispatchPayloadToServer(int serverKey, Guid publisherId ,ClientPayload payload)
    {
        var token = authService.CreateCoreServerToken();
        var authHeader = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
        var server = allServers[serverKey];
        var handler = new HttpClientHandler()
        {
            // todo: only for development
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri($"https://{server.Address}"),
            DefaultRequestHeaders = { Authorization = authHeader}
        };

        try
        {
            var result = await httpClient.PostAsync($"dispatch/{publisherId}", JsonContent.Create(payload));
            if (!result.IsSuccessStatusCode)
            {
                logger.LogError("dispatch to {server} failed with: {reason}", server.Address, result.ReasonPhrase);
            }
        }
        catch(Exception ex)
        {
            logger.LogError(ex.ToString());
        }
    }
}
