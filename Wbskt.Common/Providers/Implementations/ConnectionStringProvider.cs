﻿
using Microsoft.Extensions.Configuration;

namespace Wbskt.Common.Providers.Implementations;

internal sealed class ConnectionStringProvider : IConnectionStringProvider
{
    private readonly IConfiguration configuration;

    public ConnectionStringProvider(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        this.configuration = configuration;
    }

    public string ConnectionString => configuration["ConnectionStrings:Database"]!;
}
