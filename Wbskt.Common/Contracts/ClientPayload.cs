using System.Text.Json.Serialization;

namespace Wbskt.Common.Contracts;

public class ClientPayload
{

    [JsonIgnore]
    public int ChannelId { get; set; }
    
    public string Data { get; set; } = string.Empty;

    public Guid PublisherId { get; set; }

    public bool EnsureDelivery { get; set; }

    public Guid ChannelSubscriberId { get; set; }

    public Guid PayloadId { get; set; }
}
