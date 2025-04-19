using Microsoft.AspNetCore.Identity;
using Wbskt.Common;
using Wbskt.Common.Contracts;
using Wbskt.Common.Extensions;
using Wbskt.Common.Providers;
using Wbskt.Common.Providers.Implementations;
using Wbskt.Core.Web.Services;
using Wbskt.Core.Web.Services.Implementations;

namespace Wbskt.Core.Web;

public static class Program
{
    private static readonly CancellationTokenSource Cts = new();

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // todo: Add a new implementation that wraps PasswordHasher<User>
        builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IUsersService, UsersService>();
        builder.Services.AddSingleton<IClientService, ClientService>();
        builder.Services.AddSingleton<IChannelsService, ChannelsService>();
        builder.Services.AddSingleton<IServerInfoService, ServerInfoService>();
        // to avoid cyclic-dependency
        builder.Services.AddSingleton(sp => new Lazy<IServerInfoService>(sp.GetRequiredService<IServerInfoService>));

        builder.Services.AddSingleton<ServerHealthMonitor>();

        builder.Services.ConfigureCommonServices();

        builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = Constants.AuthSchemes.UserScheme;
                opt.DefaultChallengeScheme = Constants.AuthSchemes.UserScheme;
            })
            .AddUserAuthScheme(builder.Configuration)
            .AddClientAuthScheme(builder.Configuration)
            .AddSocketServerAuthScheme(builder.Configuration);

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
