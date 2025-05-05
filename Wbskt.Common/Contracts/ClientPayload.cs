namespace Wbskt.Common.Contracts;

public class ClientPayload
{
    public string Data { get; set; } = string.Empty;

    public Guid PublisherId { get; set; }

    public bool EnsureDelivery { get; set; }

    public Guid ChannelSubscriberId { get; set; }

    public Guid PayloadId { get; set; }
}
