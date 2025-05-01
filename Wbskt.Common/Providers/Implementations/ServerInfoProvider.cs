using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Wbskt.Common.Contracts;
using Wbskt.Common.Extensions;

namespace Wbskt.Common.Providers.Implementations;

internal sealed class ServerInfoProvider(ILogger<ServerInfoProvider> logger, IConnectionStringProvider connectionStringProvider) : IServerInfoProvider
{
    private readonly ILogger<ServerInfoProvider> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public IReadOnlyCollection<ServerInfo> GetAllServerInfo()
    {
        logger.LogDebug("DB operation: {functionName}", nameof(GetAllServerInfo));
        using var connection = new SqlConnection(connectionStringProvider.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Servers_GetAll";

        var result = new List<ServerInfo>();
        using var reader = command.ExecuteReader();
        var mapping = GetColumnMapping(reader);

        while (reader.Read()) result.Add(ParseData(reader, mapping));

        return result;
    }

    public int RegisterServer(ServerInfo serverInfo)
    {
        logger.LogDebug("DB operation: {functionName}", nameof(GetAllServerInfo));
        ArgumentNullException.ThrowIfNull(serverInfo);

        using var connection = new SqlConnection(connectionStringProvider.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Servers_Insert";

        command.Parameters.Add(new SqlParameter("@Port", ProviderExtensions.ReplaceDbNulls(serverInfo.Address.Port)));
        command.Parameters.Add(new SqlParameter("@Active", ProviderExtensions.ReplaceDbNulls(serverInfo.Active)));
        command.Parameters.Add(new SqlParameter("@IPAddress", ProviderExtensions.ReplaceDbNulls(serverInfo.Address.Host)));
        command.Parameters.Add(new SqlParameter("@PublicDomainName", ProviderExtensions.ReplaceDbNulls(serverInfo.PublicDomainName)));

        var id = new SqlParameter("@Id", SqlDbType.Int) { Size = int.MaxValue };
        id.Direction = ParameterDirection.Output;
        command.Parameters.Add(id);
        command.ExecuteNonQuery();

        return serverInfo.ServerId = (int)(ProviderExtensions.ReplaceDbNulls(id.Value) ?? 0);
    }

    public void UpdateServerStatus(int id, bool active)
    {
        logger.LogDebug("DB operation: {functionName}", nameof(GetAllServerInfo));
        using var connection = new SqlConnection(connectionStringProvider.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Servers_UpdateStatus";

        command.Parameters.Add(new SqlParameter("@Id", id));
        command.Parameters.Add(new SqlParameter("@Active", active));

        command.ExecuteNonQuery();
    }

    public void UpdatePublicDomainName(int id, string publicDomainName)
    {
        logger.LogDebug("DB operation: {functionName}", nameof(GetAllServerInfo));
        using var connection = new SqlConnection(connectionStringProvider.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Servers_UpdatePublicDomainName";

        command.Parameters.Add(new SqlParameter("@Id", id));
        command.Parameters.Add(new SqlParameter("@PublicDomainName", publicDomainName));

        command.ExecuteNonQuery();
    }

    internal void RegisterSqlDependency(OnChangeEventHandler onDatabaseChange)
    {
        try
        {
            using var connection = new SqlConnection(connectionStringProvider.ConnectionString);
            using var command = new SqlCommand("SELECT PublicDomainName, Active FROM dbo.ServerInfo", connection); // listen to changes in this output

            var dependency = new SqlDependency(command);
            dependency.OnChange += onDatabaseChange;

            connection.Open();
            using var reader = command.ExecuteReader(); // must execute to register dependency
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "failed to register SQL dependency.");
        }
    }

    private static ServerInfo ParseData(SqlDataReader reader, OrdinalColumnMapping mapping)
    {
        var address = new HostString(reader.GetString(mapping.IPAddress), reader.GetInt32(mapping.Port));
        var data = new ServerInfo
        {
            Active = reader.GetBoolean(mapping.Active),
            Address = address,
            ServerId = reader.GetInt32(mapping.ServerId),
            PublicDomainName = reader.GetString(mapping.PublicDomainName)
        };

        return data;
    }

    private static OrdinalColumnMapping GetColumnMapping(SqlDataReader reader)
    {
        var mapping = new OrdinalColumnMapping();

        mapping.Port = reader.GetOrdinal("Port");
        mapping.Active = reader.GetOrdinal("Active");
        mapping.ServerId = reader.GetOrdinal("Id");
        mapping.IPAddress = reader.GetOrdinal("IPAddress");
        mapping.PublicDomainName = reader.GetOrdinal("PublicDomainName");

        return mapping;
    }

    private class OrdinalColumnMapping
    {
        public int Port;
        public int Active;
        public int ServerId;
        public int IPAddress;
        public int PublicDomainName;
    }
}
