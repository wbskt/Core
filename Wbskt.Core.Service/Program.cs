﻿using System.Data.SqlClient;
using System.Security.Authentication;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Wbskt.Common;
using Wbskt.Common.Contracts;
using Wbskt.Common.Extensions;
using Wbskt.Common.Providers;
using Wbskt.Common.Services;
using Wbskt.Core.Service.Pipeline;
using Wbskt.Core.Service.Services;
using Wbskt.Core.Service.Services.Implementations;

namespace Wbskt.Core.Service;

public static class Program
{
    private static readonly string ProgramDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Wbskt");

    public static async Task Main(string[] args)
    {
        Environment.SetEnvironmentVariable(Constants.LoggingConstants.LogPath, ProgramDataPath);
        Environment.SetEnvironmentVariable(nameof(Constants.ServerType), Constants.ServerType.CoreServer.ToString());

        if (!Directory.Exists(ProgramDataPath))
        {
            Directory.CreateDirectory(ProgramDataPath);
        }

        var builder = WebApplication.CreateBuilder(args);

        // Configure Serilog
        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

        builder.Host.UseSerilog(Log.Logger);
        builder.WebHost.UseKestrel().ConfigureKestrel((_, options) =>
        {
            options.ConfigureHttpsDefaults(httpsOptions =>
            {
                httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
            });
        });

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
        builder.Services.AddSingleton<ICancellationService, CancellationService>();

        // to avoid cyclic-dependency
        builder.Services.AddSingleton(sp => new Lazy<IServerInfoService>(sp.GetRequiredService<IServerInfoService>));

        builder.Services.ConfigureCommonServices();

        // Register Background Services
        builder.Services.AddHostedService<ServerBackgroundService>();
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
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseWebSockets();

        app.MapControllers();

        var connectionString = app.Services.GetRequiredService<IConnectionStringProvider>().ConnectionString;
        SqlDependency.Start(connectionString);

        var serverInfoService = app.Services.GetRequiredService<IServerInfoService>();
        var cancellationService = app.Services.GetRequiredService<ICancellationService>();
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            serverInfoService.MapAllChannels();
        });

        app.Lifetime.ApplicationStopping.Register(() =>
        {
            cancellationService.Cancel().Wait();
            SqlDependency.Stop(connectionString);
        });

        await app.RunAsync(cancellationService.GetToken());
    }
}
