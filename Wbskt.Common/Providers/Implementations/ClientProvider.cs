using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Wbskt.Common.Contracts;
using Wbskt.Common.Extensions;

namespace Wbskt.Common.Providers.Implementations;

internal sealed class ClientProvider(ILogger<ClientProvider> logger, IConnectionStringProvider connectionStringProvider) : IClientProvider
{
    private readonly ILogger<ClientProvider> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public int AddOrUpdateClientConnection(ClientConnection clientConnection)
    {
        logger.LogDebug("DB operation: {functionName}", nameof(AddOrUpdateClientConnection));
        ArgumentNullException.ThrowIfNull(clientConnection);

        using var connection = new SqlConnection(connectionStringProvider.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Clients_Upsert";

        command.Parameters.Add(new SqlParameter("@Token", ProviderExtensions.ReplaceDbNulls(clientConnection.Token)));
        command.Parameters.Add(new SqlParameter("@TokenId", ProviderExtensions.ReplaceDbNulls(clientConnection.TokenId)));
        command.Parameters.Add(new SqlParameter("@ClientName", ProviderExtensions.ReplaceDbNulls(clientConnection.ClientName)));
        command.Parameters.Add(new SqlParameter("@ClientUniqueId", ProviderExtensions.ReplaceDbNulls(clientConnection.ClientUniqueId)));
        command.Parameters.Add(new SqlParameter("@ChannelSubscriberId", ProviderExtensions.ReplaceDbNulls(clientConnection.ChannelSubscriberId)));

        var id = new SqlParameter("@Id", SqlDbType.Int) { Size = int.MaxValue };
        id.Direction = ParameterDirection.Output;
        command.Parameters.Add(id);
        command.ExecuteNonQuery();

        return clientConnection.ClientId = (int)(ProviderExtensions.ReplaceDbNulls(id.Value) ?? 0);
    }

    public ClientConnection GetClientConnectionById(int clientId)
    {
        logger.LogDebug("DB operation: {functionName}", nameof(GetClientConnectionById));
        using var connection = new SqlConnection(connectionStringProvider.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Clients_GetBy_Id";

        command.Parameters.Add(new SqlParameter("@Id", clientId));

        using var reader = command.ExecuteReader();
        var mapping = GetColumnMapping(reader);
        reader.Read();
        return ParseData(reader, mapping);
    }

    public int FindClientIdByClientUniqueId(Guid clientUniqueId)
    {
        logger.LogDebug("DB operation: {functionName}", nameof(GetClientConnectionById));
        using var connection = new SqlConnection(connectionStringProvider.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Clients_FindBy_ClientUniqueId";

        command.Parameters.Add(new SqlParameter("@ClientUniqueId", clientUniqueId));

        using var reader = command.ExecuteReader();
        reader.Read();
        if (reader.HasRows)
        {
            return reader.GetInt32(reader.GetOrdinal("Id"));
        }

        return -1;
    }

    public void InvalidateToken(int clientId)
    {
        logger.LogDebug("DB operation: {functionName}", nameof(InvalidateToken));
        using var connection = new SqlConnection(connectionStringProvider.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Clients_InvalidateToken";

        command.Parameters.Add(new SqlParameter("@Id", clientId));
        command.ExecuteNonQuery();
    }

    public IReadOnlyCollection<ClientConnection> GetClientConnectionsBySubscriberId(Guid channelSubscriberId)
    {
        logger.LogDebug("DB operation: {functionName}", nameof(GetClientConnectionsBySubscriberId));
        using var connection = new SqlConnection(connectionStringProvider.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Clients_GetBy_ChannelSubscriberId";

        command.Parameters.Add(new SqlParameter("@ChannelSubscriberId", channelSubscriberId));

        var result = new List<ClientConnection>();
        using var reader = command.ExecuteReader();
        var mapping = GetColumnMapping(reader);

        while (reader.Read()) result.Add(ParseData(reader, mapping));

        return result;
    }

    private static ClientConnection ParseData(SqlDataReader reader, OrdinalColumnMapping mapping)
    {
        var data = new ClientConnection
        {
            Token = reader.GetString(mapping.Token),
            TokenId = reader.GetGuid(mapping.TokenId),
            ClientId = reader.GetInt32(mapping.ClientId),
            ClientName = reader.GetString(mapping.ClientName),
            ChannelSecret = string.Empty, // this is only used for verification. while on registration. it doesn't go to DB
            ClientUniqueId = reader.GetGuid(mapping.ClientUniqueId),
            ChannelSubscriberId = reader.GetGuid(mapping.ChannelSubscriberId),
        };

        return data;
    }

    private static OrdinalColumnMapping GetColumnMapping(SqlDataReader reader)
    {
        var mapping = new OrdinalColumnMapping();

        mapping.Token = reader.GetOrdinal("Token");
        mapping.ClientId = reader.GetOrdinal("Id");
        mapping.TokenId = reader.GetOrdinal("TokenId");
        mapping.ClientName = reader.GetOrdinal("ClientName");
        mapping.ClientUniqueId = reader.GetOrdinal("ClientUniqueId");
        mapping.ChannelSubscriberId = reader.GetOrdinal("ChannelSubscriberId");

        return mapping;
    }

    private class OrdinalColumnMapping
    {
        public int Token;
        public int TokenId;
        public int ClientId;
        public int ClientName;
        public int ClientUniqueId;
        public int ChannelSubscriberId;
    }
}
