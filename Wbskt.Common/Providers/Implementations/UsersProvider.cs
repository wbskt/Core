﻿using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Wbskt.Common.Contracts;
using Wbskt.Common.Extensions;

namespace Wbskt.Common.Providers.Implementations;

internal sealed class UsersProvider(ILogger<UsersProvider> logger, IConnectionStringProvider connectionStringProvider) : IUsersProvider
{
    private readonly ILogger<UsersProvider> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public int FindByEmailId(string emailId)
    {
        logger.LogTrace("DB operation: {functionName}", nameof(FindByEmailId));
        using var connection = new SqlConnection(connectionStringProvider.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Users_FindBy_EmailId";

        command.Parameters.Add(new SqlParameter("@EmailId", emailId));

        using var reader = command.ExecuteReader();
        reader.Read();
        if (reader.HasRows)
        {
            return reader.GetInt32(reader.GetOrdinal("Id"));
        }

        return -1;
    }

    public User? GetByEmailId(string emailId)
    {
        logger.LogTrace("DB operation: {functionName}", nameof(GetByEmailId));
        using var connection = new SqlConnection(connectionStringProvider.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Users_GetBy_EmailId";

        command.Parameters.Add(new SqlParameter("@EmailId", emailId));

        using var reader = command.ExecuteReader();
        var mapping = GetColumnMapping(reader);
        reader.Read();
        if (reader.HasRows)
        {
            return ParseData(reader, mapping);
        }

        return null;
    }

    public User? GetById(int id)
    {
        logger.LogTrace("DB operation: {functionName}", nameof(GetById));
        using var connection = new SqlConnection(connectionStringProvider.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Users_GetBy_Id";

        command.Parameters.Add(new SqlParameter("@Id", id));

        using var reader = command.ExecuteReader();
        var mapping = GetColumnMapping(reader);
        reader.Read();
        if (reader.HasRows)
        {
            return ParseData(reader, mapping);
        }

        return null;
    }

    public int Insert(User user)
    {
        logger.LogTrace("DB operation: {functionName}", nameof(Insert));
        ArgumentNullException.ThrowIfNull(user);

        using var connection = new SqlConnection(connectionStringProvider.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Users_Insert";

        command.Parameters.Add(new SqlParameter("@UserName", ProviderExtensions.ReplaceDbNulls(user.UserName)));
        command.Parameters.Add(new SqlParameter("@EmailId", ProviderExtensions.ReplaceDbNulls(user.EmailId)));
        command.Parameters.Add(new SqlParameter("@PasswordHash", ProviderExtensions.ReplaceDbNulls(user.PasswordHash)));
        command.Parameters.Add(new SqlParameter("@PasswordSalt", ProviderExtensions.ReplaceDbNulls(user.PasswordSalt)));

        var id = new SqlParameter("@Id", SqlDbType.Int) { Size = int.MaxValue };
        id.Direction = ParameterDirection.Output;
        command.Parameters.Add(id);

        command.ExecuteNonQuery();

        return user.UserId = (int)(ProviderExtensions.ReplaceDbNulls(id.Value) ?? 0);
    }

    private static User ParseData(SqlDataReader reader, OrdinalColumnMapping mapping)
    {
        var data = new User
        {
            UserId = reader.GetInt32(mapping.UserId),
            UserName = reader.GetString(mapping.UserName),
            EmailId = reader.GetString(mapping.EmailId),
            PasswordHash = reader.GetString(mapping.PasswordHash),
            PasswordSalt = reader.GetString(mapping.PasswordSalt)
        };

        return data;
    }

    private static OrdinalColumnMapping GetColumnMapping(SqlDataReader reader)
    {
        var mapping = new OrdinalColumnMapping();

        mapping.UserId = reader.GetOrdinal("Id");
        mapping.UserName = reader.GetOrdinal("UserName");
        mapping.EmailId = reader.GetOrdinal("EmailId");
        mapping.PasswordHash = reader.GetOrdinal("PasswordHash");
        mapping.PasswordSalt = reader.GetOrdinal("PasswordSalt");

        return mapping;
    }

    private class OrdinalColumnMapping
    {
        public int UserId;
        public int UserName;
        public int EmailId;
        public int PasswordHash;
        public int PasswordSalt;
    }
}
