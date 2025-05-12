using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Wbskt.Common;
using Wbskt.Common.Contracts;
using Wbskt.Common.Exceptions;
using Wbskt.Common.Providers;

namespace Wbskt.Core.Service.Services.Implementations;

public class ClientService(ILogger<ClientService> logger, IClientProvider clientProvider, IConfiguration configuration, IChannelsService channelsService, IServerInfoService serverInfoService) : IClientService
{
    private readonly ILogger<ClientService> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IClientProvider clientProvider = clientProvider ?? throw new ArgumentNullException(nameof(clientProvider));
    private readonly IConfiguration configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    private readonly IChannelsService channelsService = channelsService ?? throw new ArgumentNullException(nameof(channelsService));
    private readonly IServerInfoService serverInfoService = serverInfoService ?? throw new ArgumentNullException(nameof(serverInfoService));

    public string AddClientConnection(ClientConnectionRequest req)
    {
        if (clientProvider.Exists(req.ClientName, req.ChannelSubscriberId))
        {
            throw WbsktExceptions.ClientWithSameNameExists(req.ClientName);
        }

        // check if this client exists
        // if yes, check if its token is expired or used.
        var clientId = clientProvider.FindClientIdByClientUniqueId(req.ClientUniqueId);
        ClientConnection? conn = null;
        if (clientId > 0)
        {
            logger.LogDebug("client with uniqueId: {clientuid}, name: {name}, databaseId: {id}", req.ClientUniqueId, req.ClientName, clientId);
            var exConn = clientProvider.GetClientConnectionById(clientId);
            if (!exConn.ClientName.Equals(req.ClientName))
            {
                logger.LogWarning("mismatch between the name of the client in the DB({dbName}) and the request({reqName}). (name in the request will be discarded)", exConn.ClientName, req.ClientName);
            }

            if (exConn.TokenId == default || string.IsNullOrWhiteSpace(exConn.Token))
            {
                logger.LogDebug("client token: {tokenId} already used", exConn.TokenId);
                logger.LogDebug("creating new token for existing client: {clientuid}, name: {name}, databaseId: {id}", req.ClientUniqueId, req.ClientName, clientId);
                conn = exConn;
            }
            else if (req.ChannelSubscriberId != exConn.ChannelSubscriberId)
            {
                logger.LogDebug("existing client({name}, {id}) requesting connection to different channel subscription. previous: {prevsid}, current: {currsid}", req.ClientName, clientId, exConn.ChannelSubscriberId, req.ChannelSubscriberId);
                conn = exConn;
                conn.ChannelSubscriberId = req.ChannelSubscriberId;
            }
            else
            {
                // reuse existing token
                logger.LogDebug("reusing existing token: {tokenId}", exConn.TokenId);
                return exConn.Token;
            }
        }

        conn ??= new ClientConnection
        {
            ClientName = req.ClientName,
            ChannelSecret = req.ChannelSecret,
            ClientUniqueId = req.ClientUniqueId,
            ChannelSubscriberId = req.ChannelSubscriberId,
        };

        var channel = channelsService.GetChannelSubscriberId(conn.ChannelSubscriberId);
        var server = serverInfoService.GetServerById(channel.ServerId);
        var tokenId = Guid.NewGuid();
        conn.TokenId = tokenId;
        clientProvider.AddOrUpdateClientConnection(conn);
        var token = CreateClientToken(conn, server, tokenId);

        logger.LogDebug("token {tokenId} created for client {clientName}-{clientId}", conn.TokenId, conn.ClientName, conn.ClientUniqueId);
        return token;
    }

    private string CreateClientToken(ClientConnection conn, ServerInfo server, Guid tokenId)
    {
        var tokenHandler = new JsonWebTokenHandler();
        var configurationKey = configuration[Constants.JwtKeyNames.ClientServerTokenKey];

        var key = Encoding.UTF8.GetBytes(configurationKey!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(Constants.Claims.TokenId, tokenId.ToString()),
                new Claim(Constants.Claims.ChannelSubscriberId, conn.ChannelSubscriberId.ToString()),
                new Claim(Constants.Claims.ClientName, conn.ClientName),
                new Claim(Constants.Claims.ClientUniqueId, conn.ClientUniqueId.ToString()),
                new Claim(Constants.Claims.ClientId, conn.ClientId.ToString()),
                new Claim(Constants.Claims.SocketServer, $"{server.ServerId}|{server.GetAddressWithFallback()}")
            }),
            Expires = DateTime.UtcNow.AddMinutes(Constants.ExpiryTimes.ClientTokenExpiry),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
        };

        conn.TokenId = tokenId;
        return conn.Token = tokenHandler.CreateToken(tokenDescriptor);
    }
}
