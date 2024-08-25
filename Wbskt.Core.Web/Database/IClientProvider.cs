using Wbskt.Core.Web.Services;

namespace Wbskt.Core.Web.Database
{
    public interface IClientProvider
    {
        ClientConenction GetClientConenctionByUId(string clientUniqueId);

        IReadOnlyCollection<ClientConenction> GetClientConenctionsBySubcriberId(Guid channelSubcriberId);

        int AddClientConnection(ClientConenction clientConenction);
    }
}
