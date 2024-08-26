using System.Data;
using System.Data.SqlClient;
using Wbskt.Core.Web.Services;

namespace Wbskt.Core.Web.Database.Providers
{
    public class ServerInfoProvider : IServerInfoProvider
    {
        private readonly ILogger<ServerInfoProvider> logger;
        private readonly string _connectionString;

        public ServerInfoProvider(ILogger<ServerInfoProvider> logger, IConnectionStringProvider connectionStringProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionString = connectionStringProvider?.ConnectionString ?? throw new ArgumentNullException(nameof(connectionStringProvider));
        }

        public IReadOnlyCollection<ServerInfo> GetAll()
        {
            using var connection = new SqlConnection(_connectionString);
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

        public void UpdateServerStatus(int id, bool active)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.Servers_UpdateStatus";

            command.Parameters.Add(new SqlParameter("@Id", id));
            command.Parameters.Add(new SqlParameter("@Active", active));

            var result = new List<ServerInfo>();
            command.ExecuteNonQuery();
        }

        private static ServerInfo ParseData(IDataRecord reader, OrdinalColumnMapping mapping)
        {
            var address = new HostString(reader.GetString(mapping.IPAddress), reader.GetInt32(mapping.Port));
            var data = new ServerInfo
            {
                ServerId = reader.GetInt32(mapping.ServerId),
                Active = reader.GetBoolean(mapping.Active),
                Address = address
            };

            return data;
        }

        private static OrdinalColumnMapping GetColumnMapping(IDataRecord reader)
        {
            var mapping = new OrdinalColumnMapping();

            mapping.ServerId = reader.GetOrdinal("Id");
            mapping.IPAddress = reader.GetOrdinal("IPAddress");
            mapping.Port = reader.GetOrdinal("Port");
            mapping.Active = reader.GetOrdinal("Active");

            return mapping;
        }

        private class OrdinalColumnMapping
        {
            public int ServerId;
            public int IPAddress;
            public int Port;
            public int Active;
        }
    }
}
