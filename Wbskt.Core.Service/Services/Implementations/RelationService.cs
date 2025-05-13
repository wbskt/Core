using System.Collections.Concurrent;
using Wbskt.Common.Contracts;
using Wbskt.Common.Exceptions;
using Wbskt.Common.Providers;

namespace Wbskt.Core.Service.Services.Implementations;

public interface IRelationService
{
    int GetAvailableServerId();
    void InitializeRelations();
}

public class RelationService(ILogger<RelationService> logger, IClientProvider clientProvider, ICachedServerInfoProvider serverInfoProvider, ICachedChannelsProvider channelsProvider) : IRelationService
{
    // server - [channels]
    // used for: re-balancing when a particulate server becomes offline
    private readonly ConcurrentDictionary<int, ConcurrentBag<int>> serverChannelsMap = new();

    // channel - [servers]
    // used for: dispatching messages to servers based on the channelId (gets the list of servers to which a message needs to be dispatched for a channel)
    private readonly ConcurrentDictionary<int, ConcurrentBag<int>> channelServersMap = new();
    //<------------------------------->

    // client - [server]
    // used for: dispatching messages to particular client. A client will always be assigned to one server.
    private readonly ConcurrentDictionary<int, int> clientServerMap = new();

    // server - [client]
    // used for: health purposes and re-balancing. to know how many clients are assigned to a particular server.
    private readonly ConcurrentDictionary<int, ConcurrentBag<int>> serverClientsMap = new();
    //<------------------------------->

    // channel - [clients]
    // used for: validation, n clients per channel, and look up for re-balancing
    private readonly ConcurrentDictionary<int, ConcurrentBag<int>> channelClientsMap = new();

    public int GetAvailableServerId()
    {
        if (serverClientsMap.IsEmpty)
        {
            throw WbsktExceptions.SocketServerUnavailable();
        }

        var serverId = serverClientsMap.MinBy(m => m.Value?.Count ?? 0).Key;
        logger.LogDebug("available serverId: {serverId}", serverId);
        return serverId;
    }

    #region Initialization
    public void InitializeRelations()
    {
        var servers = serverInfoProvider.GetAllSocketServerInfo();
        var channels = channelsProvider.GetAll();
        var clients = clientProvider.GetAll();
        MapAllServerChannels(servers);
        MapAllChannelClients(channels, clients);
        MapAllClientServers(clients, servers);
    }

    private void MapAllServerChannels(IReadOnlyCollection<ServerInfo> servers)
    {
        serverChannelsMap.Clear();
        channelServersMap.Clear();
        foreach (var server in servers)
        {
            var channelIds = new ConcurrentBag<int>();
            foreach (var channel in channelsProvider.GetAllByServerIds([server.ServerId]))
            {
                channelIds.Add(channel.ChannelId);

                if (channelServersMap.TryGetValue(channel.ChannelId, out var serverIds))
                {
                    serverIds.Add(server.ServerId);
                }
                else
                {
                    if (!channelServersMap.TryAdd(channel.ChannelId, new ConcurrentBag<int>([server.ServerId])))
                    {
                        logger.LogError("couldn't add value to 'channelServersMap'");
                    }
                }
            }

            serverChannelsMap.TryAdd(server.ServerId, channelIds);
        }
    }

    private void MapAllChannelClients(IReadOnlyCollection<ChannelDetails> channels, IReadOnlyCollection<ClientConnection> clients)
    {
        channelClientsMap.Clear();
        foreach (var channel in channels)
        {
            var clientIds = new ConcurrentBag<int>();
            foreach (var client in clients.Where(c => c.Channels.Select(chan => chan.ChannelSubscriberId).Contains(channel.ChannelSubscriberId)))
            {
                clientIds.Add(client.ClientId);
            }

            channelClientsMap.TryAdd(channel.ChannelId, clientIds);
        }
    }

    private void MapAllClientServers(IReadOnlyCollection<ClientConnection> clients, IReadOnlyCollection<ServerInfo> servers)
    {
        serverClientsMap.Clear();
        clientServerMap.Clear();
        foreach (var server in servers)
        {
            var clientIds = new ConcurrentBag<int>();
            foreach (var client in clients.Where(c => c.ServerId == server.ServerId))
            {
                clientIds.Add(client.ClientId);
                clientServerMap.TryAdd(client.ClientId, server.ServerId);
            }

            serverClientsMap.TryAdd(server.ServerId, clientIds);
        }
    }
    #endregion
}
