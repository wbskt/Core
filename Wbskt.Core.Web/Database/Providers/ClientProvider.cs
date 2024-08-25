using System.Data;
using System.Data.SqlClient;
using Wbskt.Core.Web.Services;

namespace Wbskt.Core.Web.Database.Providers
{
    public class ClientProvider : IClientProvider
    {
        private readonly ILogger<ClientProvider> logger;
        private readonly string _connectionString;

        public ClientProvider(ILogger<ClientProvider> logger, IConnectionStringProvider connectionStringProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionString = connectionStringProvider?.ConnectionString ?? throw new ArgumentNullException(nameof(connectionStringProvider));
        }

        public int AddClientConnection(ClientConenction clientConenction)
        {
            if (clientConenction == null)
                throw new ArgumentNullException(nameof(clientConenction));

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.Clients_Insert";

            command.Parameters.Add(new SqlParameter("@TokenId", ProviderExtensions.ReplaceDbNulls(clientConenction.TokenId)));
            command.Parameters.Add(new SqlParameter("@Disabled", ProviderExtensions.ReplaceDbNulls(clientConenction.Disabled)));
            command.Parameters.Add(new SqlParameter("@ClientName", ProviderExtensions.ReplaceDbNulls(clientConenction.ClientName)));
            command.Parameters.Add(new SqlParameter("@ClientUniqueId", ProviderExtensions.ReplaceDbNulls(clientConenction.ClientUniqueId)));
            command.Parameters.Add(new SqlParameter("@ChannelSubscriberId", ProviderExtensions.ReplaceDbNulls(clientConenction.ChannelSubscriberId)));

            var id = new SqlParameter("@Id", SqlDbType.Int) { Size = int.MaxValue };
            id.Direction = ParameterDirection.Output;
            command.Parameters.Add(id);
            command.ExecuteNonQuery();

            return clientConenction.ClientId = (int)(ProviderExtensions.ReplaceDbNulls(id.Value) ?? 0);
        }

        public ClientConenction GetClientConenctionByUId(string clientUniqueId)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<ClientConenction> GetClientConenctionsBySubcriberId(Guid channelSubcriberId)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "dbo.Clients_GetBy_ChannelSubscriberId";

            command.Parameters.Add(new SqlParameter("@ChannelSubscriberId", channelSubcriberId));

            var result = new List<ClientConenction>();
            using var reader = command.ExecuteReader();
            var mapping = GetColumnMapping(reader);

            while (reader.Read()) result.Add(ParseData(reader, mapping));

            return result;
        }

        private static ClientConenction ParseData(IDataRecord reader, OrdinalColumnMapping mapping)
        {
            var data = new ClientConenction
            {
                TokenId = reader.GetGuid(mapping.TokenId),
                Disabled = reader.GetBoolean(mapping.Disabled),
                ClientId = reader.GetInt32(mapping.ClientId),
                ClientName = reader.GetString(mapping.ClientName),
                ClientUniqueId = reader.GetString(mapping.ClientUniqueId),
                ChannelSubscriberId = reader.GetGuid(mapping.ChannelSubscriberId),
            };

            return data;
        }

        private static OrdinalColumnMapping GetColumnMapping(IDataRecord reader)
        {
            var mapping = new OrdinalColumnMapping();

            mapping.ClientId = reader.GetOrdinal("Id");
            mapping.TokenId = reader.GetOrdinal("TokenId");
            mapping.Disabled = reader.GetOrdinal("Disabled");
            mapping.ClientName = reader.GetOrdinal("ClientName");
            mapping.ClientUniqueId = reader.GetOrdinal("ClientUniqueId");
            mapping.ChannelSubscriberId = reader.GetOrdinal("ChannelSubscriberId");

            return mapping;
        }

        private class OrdinalColumnMapping
        {
            public int TokenId;
            public int Disabled;
            public int ClientId;
            public int ClientName;
            public int ClientUniqueId;
            public int ChannelSubscriberId;
        }
    }
}
