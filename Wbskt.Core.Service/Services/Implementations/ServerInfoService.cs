using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Wbskt.Common.Contracts;
using Wbskt.Common.Providers;

namespace Wbskt.Core.Service.Services.Implementations;

public class ServerInfoService(ILogger<ServerInfoService> logger, ICachedServerInfoProvider serverInfoProvider, ICachedChannelsProvider channelsService, IAuthService authService) : IServerInfoService
{
    /// <summary>
    /// Map of each S.S with the list of channels assigned to it.
    /// </summary>
    private readonly IDictionary<int, HashSet<int>> serverChannelMap = new Dictionary<int, HashSet<int>>();
    private IDictionary<int, ServerInfo> allServers = new Dictionary<int, ServerInfo>();
    private readonly ILogger<ServerInfoService> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ICachedServerInfoProvider serverInfoProvider = serverInfoProvider ?? throw new ArgumentNullException(nameof(serverInfoProvider));
    private readonly ICachedChannelsProvider channelsService = channelsService ?? throw new ArgumentNullException(nameof(channelsService));
    private readonly IAuthService authService = authService ?? throw new ArgumentNullException(nameof(authService));


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
                    tasks.Add(DispatchPayloadToServer(serverChannel.Key, payload));
                }
            }
            await Task.WhenAll(tasks);
            return true;
        }

        return false;
    }

    public void UpdateServerStatus(int id, bool active)
    {
        logger.LogDebug("updating status of server: {serverId} - {active}", id, active);
        serverInfoProvider.UpdateServerStatus(id, active);
    }

    private async Task DispatchPayloadToServer(int serverKey, ClientPayload payload)
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
            logger.LogDebug("post: {url}/dispatch with publisher id: {publisherId}", httpClient.BaseAddress, payload.PublisherId);
            var result = await httpClient.PostAsync($"dispatch", JsonContent.Create(payload));
            if (!result.IsSuccessStatusCode)
            {
                logger.LogError("dispatch to {server} Id:({serverId}) failed with: {reason}", server.GetAddressWithFallback(), server.ServerId, result.ReasonPhrase);
            }
        }
        catch(Exception ex)
        {
            logger.LogWarning("error while request to {url}/dispatch/{publisher}, error: {details}", httpClient.BaseAddress, payload.PublisherId, ex.Message);
        }
    }
}
