using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Wbskt.Common.Contracts;
using Wbskt.Common.Extensions;

namespace Wbskt.Common.Providers.Implementations;

internal sealed class ChannelsProvider(ILogger<ChannelsProvider> logger, IConnectionStringProvider connectionStringProvider) : IChannelsProvider
{
    private readonly ILogger<ChannelsProvider> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public int CreateChannel(ChannelDetails channel)
    {
        logger.LogDebug("DB operation: {functionName}", nameof(CreateChannel));
        ArgumentNullException.ThrowIfNull(channel);

        using var connection = new SqlConnection(connectionStringProvider.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Channels_Insert";

        command.Parameters.Add(new SqlParameter("@UserId", ProviderExtensions.ReplaceDbNulls(channel.UserId)));
        command.Parameters.Add(new SqlParameter("@ServerId", ProviderExtensions.ReplaceDbNulls(channel.ServerId)));
        command.Parameters.Add(new SqlParameter("@ChannelName", ProviderExtensions.ReplaceDbNulls(channel.ChannelName)));
        command.Parameters.Add(new SqlParameter("@ChannelSecret", ProviderExtensions.ReplaceDbNulls(channel.ChannelSecret)));
        command.Parameters.Add(new SqlParameter("@RetentionTime", ProviderExtensions.ReplaceDbNulls(channel.RetentionTime)));
        command.Parameters.Add(new SqlParameter("@ChannelPublisherId", ProviderExtensions.ReplaceDbNulls(channel.ChannelPublisherId)));
        command.Parameters.Add(new SqlParameter("@ChannelSubscriberId", ProviderExtensions.ReplaceDbNulls(channel.ChannelSubscriberId)));

        var id = new SqlParameter("@Id", SqlDbType.Int) { Size = int.MaxValue };
        id.Direction = ParameterDirection.Output;
        command.Parameters.Add(id);

        command.ExecuteNonQuery();

        return channel.ChannelId = (int)(ProviderExtensions.ReplaceDbNulls(id.Value) ?? 0);
    }

    public void UpdateServerId(int channelId, int serverId)
    {
        logger.LogDebug("DB operation: {functionName}", nameof(UpdateServerId));
        using var connection = new SqlConnection(connectionStringProvider.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Channels_ServerId_UpdateBy_Id";

        command.Parameters.Add(new SqlParameter("@Id", ProviderExtensions.ReplaceDbNulls(channelId)));
        command.Parameters.Add(new SqlParameter("@ServerId", ProviderExtensions.ReplaceDbNulls(serverId)));

        command.ExecuteNonQuery();
    }

    public IReadOnlyCollection<ChannelDetails> GetChannelsByUser(int userId)
    {
        logger.LogDebug("DB operation: {functionName}", nameof(GetChannelsByUser));
        using var connection = new SqlConnection(connectionStringProvider.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Channels_GetBy_UserId";

        command.Parameters.Add(new SqlParameter("@UserId", userId));

        var result = new List<ChannelDetails>();
        using var reader = command.ExecuteReader();
        var mapping = GetColumnMapping(reader);

        while (reader.Read()) result.Add(ParseData(reader, mapping));

        return result;
    }

    public IReadOnlyCollection<ChannelDetails> GetAll()
    {
        logger.LogDebug("DB operation: {functionName}", nameof(GetAll));
        using var connection = new SqlConnection(connectionStringProvider.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Channels_GetAll";

        var result = new List<ChannelDetails>();
        using var reader = command.ExecuteReader();
        var mapping = GetColumnMapping(reader);

        while (reader.Read()) result.Add(ParseData(reader, mapping));

        return result;
    }

    public ChannelDetails GetChannelSubscriberId(Guid channelSubscriberId)
    {
        logger.LogDebug("DB operation: {functionName}", nameof(GetChannelSubscriberId));
        using var connection = new SqlConnection(connectionStringProvider.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Channels_GetBy_ChannelSubscriberId";

        command.Parameters.Add(new SqlParameter("@ChannelSubscriberId", channelSubscriberId));

        using var reader = command.ExecuteReader();
        var mapping = GetColumnMapping(reader);
        reader.Read();
        return ParseData(reader, mapping);
    }

    public IReadOnlyCollection<ChannelDetails> GetChannelPublisherId(Guid channelPublisherId)
    {
        logger.LogDebug("DB operation: {functionName}", nameof(GetChannelPublisherId));
        using var connection = new SqlConnection(connectionStringProvider.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Channels_GetBy_ChannelPublisherId";

        command.Parameters.Add(new SqlParameter("@ChannelPublisherId", channelPublisherId));

        var result = new List<ChannelDetails>();
        using var reader = command.ExecuteReader();
        var mapping = GetColumnMapping(reader);

        while (reader.Read()) result.Add(ParseData(reader, mapping));

        return result;
    }

    private static ChannelDetails ParseData(SqlDataReader reader, OrdinalColumnMapping mapping)
    {
        var data = new ChannelDetails
        {
            UserId = reader.GetInt32(mapping.UserId),
            ServerId = reader.GetInt32(mapping.ServerId),
            ChannelId = reader.GetInt32(mapping.ChannelId),
            ChannelName = reader.GetString(mapping.ChannelName),
            RetentionTime = reader.GetInt32(mapping.RetentionTime),
            ChannelSecret = reader.GetString(mapping.ChannelSecret),
            ChannelPublisherId = reader.GetGuid(mapping.ChannelPublisherId),
            ChannelSubscriberId = reader.GetGuid(mapping.ChannelSubscriberId)
        };

        return data;
    }

    private static OrdinalColumnMapping GetColumnMapping(SqlDataReader reader)
    {
        var mapping = new OrdinalColumnMapping();

        mapping.UserId = reader.GetOrdinal("UserId");
        mapping.ServerId = reader.GetOrdinal("ServerId");
        mapping.ChannelId = reader.GetOrdinal("Id");
        mapping.ChannelName = reader.GetOrdinal("ChannelName");
        mapping.RetentionTime = reader.GetOrdinal("RetentionTime");
        mapping.ChannelSecret = reader.GetOrdinal("ChannelSecret");
        mapping.ChannelPublisherId = reader.GetOrdinal("ChannelPublisherId");
        mapping.ChannelSubscriberId = reader.GetOrdinal("ChannelSubscriberId");

        return mapping;
    }

    private class OrdinalColumnMapping
    {
        public int UserId;
        public int ServerId;
        public int ChannelId;
        public int ChannelName;
        public int RetentionTime;
        public int ChannelSecret;
        public int ChannelPublisherId;
        public int ChannelSubscriberId;
    }
}
