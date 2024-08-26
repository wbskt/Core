using Wbskt.Core.Web.Services;

namespace Wbskt.Core.Web.Database
{
    public interface IClientProvider
    {
        ClientConenction GetClientConenctionById(int clientId);

        IReadOnlyCollection<ClientConenction> GetClientConenctionsBySubcriberId(Guid channelSubcriberId);

        int AddClientConnection(ClientConenction clientConenction);

        void InvalidateToken(int clientId);
    }
}
