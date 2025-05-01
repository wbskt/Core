namespace Wbskt.Common.Contracts;

public class SocketServerHealth
{
    public Connection[] ActiveConnections { get; set; } = [];
}

public class Connection
{
    public string ClientName { get; set; } = string.Empty;

    public string ChannelName { get; set; } = string.Empty;
}


