using Microsoft.AspNetCore.Identity;
using Wbskt.Core.Web.Database;
using Wbskt.Core.Web.Database.Providers;
using Wbskt.Core.Web.Services;
using Wbskt.Core.Web.Services.Implementations;

namespace Wbskt.Core.Web
{
    public class Program
    {
        private static readonly CancellationTokenSource Cts = new();

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
            
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IUsersService, UsersService>();
            builder.Services.AddSingleton<IClientService, ClientService>();
            builder.Services.AddSingleton<IChannelsService, ChannelsService>();
            builder.Services.AddSingleton<IServerInfoService, ServerInfoService>();
            // to avoid cyclic-dependancy
            builder.Services.AddSingleton(sp => new Lazy<IServerInfoService>(() => sp.GetRequiredService<IServerInfoService>()));

            builder.Services.AddSingleton<ServerHealthMonitor>();

            builder.Services.AddSingleton<IUsersProvider, UsersProvider>();
            builder.Services.AddSingleton<IClientProvider, ClientProvider>();
            builder.Services.AddSingleton<IChannelsProvider, ChannelsProvider>();
            builder.Services.AddSingleton<IServerInfoProvider, ServerInfoProvider>();

            builder.Services.AddSingleton<IConnectionStringProvider, ConnectionStringProvider>();

            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.Lifetime.ApplicationStopping.Register(Cts.Cancel);
            app.Lifetime.ApplicationStarted.Register(() => app.Services.GetRequiredService<ServerHealthMonitor>());

            app.MapControllers();

            app.Run();
        }
    }
}
