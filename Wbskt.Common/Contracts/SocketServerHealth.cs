namespace Wbskt.Common.Contracts;

public class SocketServerHealth
{
    public Connection[] ActiveConnections { get; set; }
}

public class Connection
{
    public string ClientName { get; set; }

    public string ChannelName { get; set; }
}


