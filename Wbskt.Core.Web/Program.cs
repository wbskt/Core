using Microsoft.AspNetCore.Identity;
using Wbskt.Core.Web.Database;
using Wbskt.Core.Web.Database.Providers;
using Wbskt.Core.Web.Services;
using Wbskt.Core.Web.Services.Implementations;

namespace Wbskt.Core.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
            
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IUsersService, UsersService>();
            builder.Services.AddSingleton<IChannelsService, ChannelsService>();

            builder.Services.AddSingleton<IUsersProvider, UsersProvider>();
            builder.Services.AddSingleton<IChannelsProvider, ChannelsProvider>();
            builder.Services.AddSingleton<IConnectionStringProvider, ConnectionStringProvider>();

            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
