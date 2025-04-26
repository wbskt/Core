using Microsoft.AspNetCore.Identity;
using Serilog;
using Wbskt.Common;
using Wbskt.Common.Contracts;
using Wbskt.Common.Extensions;
using Wbskt.Core.Web.Services;
using Wbskt.Core.Web.Services.Implementations;

namespace Wbskt.Core.Web;

public static class Program
{
    private static readonly string ProgramDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Wbskt");

    public static void Main(string[] args)
    {
        Environment.SetEnvironmentVariable("LogPath", ProgramDataPath);
        if (!Directory.Exists(ProgramDataPath))
        {
            Directory.CreateDirectory(ProgramDataPath);
        }

        var builder = WebApplication.CreateBuilder(args);

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Host.UseSerilog(Log.Logger);

        // Add Windows Service hosting
        builder.Host.UseWindowsService();

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

        // Register Background Services
        builder.Services.AddHostedService<ServerHealthMonitorBackgroundService>();
        builder.Services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = Constants.AuthSchemes.UserScheme;
                opt.DefaultChallengeScheme = Constants.AuthSchemes.UserScheme;
            })
            .AddUserAuthScheme(builder.Configuration)
            .AddClientAuthScheme(builder.Configuration)
            .AddSocketServerAuthScheme(builder.Configuration);

        builder.Services.AddAuthorization();

        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
