namespace Wbskt.Core.Service.Services.Implementations;

public interface IRelationService
{
    int GetAvailableServerId();
    void InitializeRelations();
    void AssignClientToServer(int clientId, int serverId);
    void SetClientChannels(int clientId, int[] channelIds);
    void RemoveServerMappings(int serverId);
}
