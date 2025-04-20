using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Wbskt.Common.Contracts;
using Wbskt.Common.Extensions;

namespace Wbskt.Common.Providers.Implementations;

internal sealed class UsersProvider(ILogger<UsersProvider> logger, IConnectionStringProvider connectionStringProvider) : IUsersProvider
{
    private readonly ILogger<UsersProvider> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly string connectionString = connectionStringProvider?.ConnectionString ?? throw new ArgumentNullException(nameof(connectionStringProvider));

    public int AddUser(User user)
    {
        logger.LogDebug("DB operation: {functionName}", nameof(AddUser));
        ArgumentNullException.ThrowIfNull(user);

        using var connection = new SqlConnection(connectionString);
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

    public User GetUserById(int id)
    {
        logger.LogDebug("DB operation: {functionName}", nameof(GetUserById));
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Users_GetBy_Id";

        command.Parameters.Add(new SqlParameter("@Id", id));

        using var reader = command.ExecuteReader();
        var mapping = GetColumnMapping(reader);
        reader.Read();
        return ParseData(reader, mapping);
    }

    public User GetUserByEmailId(string emailId)
    {
        logger.LogDebug("DB operation: {functionName}", nameof(GetUserByEmailId));
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "dbo.Users_GetBy_EmailId";

        command.Parameters.Add(new SqlParameter("@EmailId", emailId));

        using var reader = command.ExecuteReader();
        var mapping = GetColumnMapping(reader);
        reader.Read();
        return ParseData(reader, mapping);
    }

    private static User ParseData(IDataRecord reader, OrdinalColumnMapping mapping)
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

    private static OrdinalColumnMapping GetColumnMapping(IDataRecord reader)
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
