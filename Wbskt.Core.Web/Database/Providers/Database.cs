namespace Wbskt.Core.Web.Database.Providers
{
    public class Database : IDatabase
    {
        public Database(IConfiguration configuration)
        {
            ConnectionString = configuration["ConnectionStrings:database"]!;
        }

        public string ConnectionString { get; set; }
    }
}
