using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Wbskt.Common;
using Wbskt.Common.Contracts;
using Wbskt.Common.Providers;

namespace Wbskt.Core.Web.Services.Implementations;

public class ClientService(ILogger<ClientService> logger, IClientProvider clientProvider, IConfiguration configuration, IChannelsService channelsService, IServerInfoService serverInfoService) : IClientService
{
    private readonly ILogger<ClientService> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IClientProvider clientProvider = clientProvider ?? throw new ArgumentNullException(nameof(clientProvider));
    private readonly IConfiguration configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    private readonly IChannelsService channelsService = channelsService ?? throw new ArgumentNullException(nameof(channelsService));
    private readonly IServerInfoService serverInfoService = serverInfoService ?? throw new ArgumentNullException(nameof(serverInfoService));

    public string AddClientConnection(ClientConnectionRequest request)
    {
        var tokenHandler = new JsonWebTokenHandler();
        var configurationKey = configuration[Constants.JwtKeyNames.ClientServerTokenKey];
        var tokenId = Guid.NewGuid();

        var connectionData = new ClientConnection
        {
            TokenId = tokenId,
            ClientName = request.ClientName,
            ChannelSecret = request.ChannelSecret,
            ClientUniqueId = request.ClientUniqueId,
            ChannelSubscriberId = request.ChannelSubscriberId,
        };

        var channel = channelsService.GetChannelSubscriberId(request.ChannelSubscriberId);
        var server = serverInfoService.GetServerById(channel.ServerId);
        connectionData.ClientId = clientProvider.AddOrUpdateClientConnection(connectionData);

        var key = Encoding.UTF8.GetBytes(configurationKey!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(Constants.Claims.TokenId, tokenId.ToString()),
                new Claim(Constants.Claims.ChannelSubscriberId, connectionData.ChannelSubscriberId.ToString()),
                new Claim(Constants.Claims.ClientName, connectionData.ClientName),
                new Claim(Constants.Claims.ClientId, connectionData.ClientId.ToString()),
                new Claim(Constants.Claims.SocketServer, $"{server.ServerId}:{server.Address}") // todo: bit wonky
            }),
            Expires = DateTime.UtcNow.AddMinutes(Constants.ExpiryTimes.ClientTokenExpiry),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
        };

        return tokenHandler.CreateToken(tokenDescriptor);
    }
}
