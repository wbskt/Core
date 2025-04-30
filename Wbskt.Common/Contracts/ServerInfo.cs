using Microsoft.AspNetCore.Http;

namespace Wbskt.Common.Contracts;

public class ServerInfo
{
    public int ServerId { get; set; }

    public HostString Address { get; set; }

    public bool Active { get; set; }

    public string PublicDomainName  { get; set; } = string.Empty;
}
