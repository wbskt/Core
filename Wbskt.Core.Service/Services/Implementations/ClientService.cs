﻿using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Wbskt.Common;
using Wbskt.Common.Contracts;
using Wbskt.Common.Exceptions;
using Wbskt.Common.Providers;

namespace Wbskt.Core.Service.Services.Implementations;

public class ClientService(ILogger<ClientService> logger, IClientProvider clientProvider, IConfiguration configuration, ICachedChannelsProvider channelsProvider, ICachedServerInfoProvider serverInfoProvider, IRelationService relationService) : IClientService
{
    public string AddClientConnection(ClientConnectionRequest req)
    {
        var reqSubIds = req.Channels.Select(c => c.ChannelSubscriberId).ToArray();
        var channels = channelsProvider.GetAllByChannelSubscriberIds(reqSubIds);
        if (channels.Select(c => c.UserId).Distinct().Count() > 1)
        {
            throw WbsktExceptions.UnauthorizedAccessToChannels();
        }

        ClientConnection? conn = null;

        // check if this client exists
        var clientId = clientProvider.FindByClientUniqueId(req.ClientUniqueId);
        ServerInfo? server;
        if (clientId > 0)
        {
            // client exists
            var exConn = clientProvider.GetByClientId(clientId)!;
            conn = exConn;

            var exSubIds = exConn.Channels.Select(c => c.ChannelSubscriberId).ToArray();
            var union = reqSubIds.Union(exSubIds).ToArray();

            var ids = channelsProvider.GetAllByChannelSubscriberIds(union).Select(c => c.ChannelId).ToArray();
            if (union.Length != exSubIds.Length)
            {
                clientProvider.SetClientChannels(exConn.ClientId, ids);
            }
            // else - no need of any db updates

            if (exConn.ClientName != req.ClientName)
            {
                logger.LogWarning("mismatch between the name of the client in the DB({dbName}) and the request({reqName}). (name in the DB will be updated)", exConn.ClientName, req.ClientName);
                conn.ClientName = req.ClientName;
                clientProvider.Upsert(conn); // updates only the name. todo: remove upsert, use dedicated sp for name updation
            }

            // todo: get available servers
            // think: this may lead to multiple connections with different servers if not properly re worked.
            // scenarios: server unreachable(maybe some network issue or an overload, in this case the data in SS persists), server down(SS data lost)
            var serverId = relationService.GetAvailableServerId();
            server = serverInfoProvider.GetById(serverId);
            return CreateClientToken(conn, server, ids);
        }

        conn ??= new ClientConnection
        {
            ClientName = req.ClientName,
            ClientUniqueId = req.ClientUniqueId,
            ServerId = relationService.GetAvailableServerId()
        };

        var channelIds = channels.Select(c => c.ChannelId).ToArray();

        clientProvider.Upsert(conn);
        clientProvider.SetClientChannels(conn.ClientId, channelIds);

        server = serverInfoProvider.GetById(conn.ServerId);
        var token = CreateClientToken(conn, server, channelIds);

        return token;
    }

    private string CreateClientToken(ClientConnection conn, ServerInfo server, int[] channelIds)
    {
        var tokenId = Guid.NewGuid();
        var tokenHandler = new JsonWebTokenHandler();
        var configurationKey = configuration[Constants.JwtKeyNames.ClientServerTokenKey];

        var key = Encoding.UTF8.GetBytes(configurationKey!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(Constants.Claims.TokenId, tokenId.ToString()),
                new Claim(Constants.Claims.ChannelIds, string.Join(",", channelIds)),
                new Claim(Constants.Claims.ClientName, conn.ClientName),
                new Claim(Constants.Claims.ClientUniqueId, conn.ClientUniqueId.ToString()),
                new Claim(Constants.Claims.ClientId, conn.ClientId.ToString()),
                new Claim(Constants.Claims.SocketServer, $"{server.ServerId}|{server.GetAddressWithFallback()}")
            }),
            Expires = DateTime.UtcNow.AddMinutes(Constants.ExpiryTimes.ClientTokenExpiry),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
        };

        logger.LogDebug("token {tokenId} created for client {clientName}-{clientId}", tokenId, conn.ClientName, conn.ClientUniqueId);
        return tokenHandler.CreateToken(tokenDescriptor);
    }
}
