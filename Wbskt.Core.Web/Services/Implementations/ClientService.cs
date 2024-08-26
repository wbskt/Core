using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Wbskt.Core.Web.Database;

namespace Wbskt.Core.Web.Services.Implementations
{
    public class ClientService : IClientService
    {
        private readonly ILogger<ClientService> logger;
        private readonly IClientProvider clientProvider;
        private readonly IConfiguration configuration;
        private readonly IChannelsService channelsService;
        private readonly IServerInfoService serverInfoService;

        public ClientService(ILogger<ClientService> logger, IClientProvider clientProvider, IConfiguration configuration, IChannelsService channelsService, IServerInfoService serverInfoService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.clientProvider = clientProvider ?? throw new ArgumentNullException(nameof(clientProvider));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.channelsService = channelsService ?? throw new ArgumentNullException(nameof(channelsService));
            this.serverInfoService = serverInfoService ?? throw new ArgumentNullException(nameof(serverInfoService));
        }

        public string RegisterClientConnection(ClientConnectionRequest request)
        {
            var tokenHandler = new JsonWebTokenHandler();
            var configurationKey = configuration["Jwt:ClientKey"];
            var tokenId = Guid.NewGuid();

            var connectionData = new ClientConenction
            {
                ClientName = request.ClientName,
                ClientUniqueId = request.ClientUniqueId,
                ChannelSubscriberId = request.ChannelSubscriberId,
                TokenId = tokenId,
            };

            var channel = channelsService.GetChannelSubscriberId(request.ChannelSubscriberId);
            var server = serverInfoService.GetServerById(channel.ServerId);
            connectionData.ClientId = clientProvider.AddClientConnection(connectionData);

            var key = Encoding.UTF8.GetBytes(configurationKey!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("tid", tokenId.ToString()),
                    new Claim("csid", connectionData.ChannelSubscriberId.ToString()),
                    new Claim("name", connectionData.ClientName.ToString()),
                    new Claim("cid", connectionData.ClientId.ToString()),
                    new Claim("sad", $"{server.ServerId}:{server.Address}")
                }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            };

            return tokenHandler.CreateToken(tokenDescriptor);
        }
    }
}
