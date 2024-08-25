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

        public ClientService(ILogger<ClientService> logger, IClientProvider clientProvider, IConfiguration configuration)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.clientProvider = clientProvider ?? throw new ArgumentNullException(nameof(clientProvider));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string RegisterClientConnection(ClientConnectionRequest request)
        {
            var tokenHandler = new JsonWebTokenHandler();
            var configurationKey = configuration["Jwt:ClientKey"];
            var tokenId = Guid.NewGuid();

            var key = Encoding.UTF8.GetBytes(configurationKey!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("tid", tokenId.ToString()),
                    new Claim("csid", request.ChannelSubscriberId.ToString()),
                    new Claim("name", request.ClientName.ToString()),
                    new Claim("uid", request.ClientUniqueId),
                }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Issuer = configuration["Jwt:ValidIssuer"],
                Audience = configuration["Jwt:ValidAudience"]
            };

            var connectionData = new ClientConenction
            {
                ClientName = request.ClientName,
                ClientUniqueId = request.ClientUniqueId,
                ChannelSubscriberId = request.ChannelSubscriberId,
                TokenId = tokenId,
            };

            connectionData.ClientId = clientProvider.AddClientConnection(connectionData);

            return tokenHandler.CreateToken(tokenDescriptor);
        }
    }
}
