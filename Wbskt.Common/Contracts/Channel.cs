using System.Text.Json.Serialization;

namespace Wbskt.Common.Contracts;

public class ChannelDetails : ChannelCreationRequest
{
    /// <summary>
    /// [Internal] ID
    /// </summary>
    [JsonIgnore]
    public int ChannelId { get; set; }

    /// <summary>
    /// [Internal]
    /// Used for a client to connect to a channel
    /// </summary>
    public Guid ChannelSubscriberId { get; set; }
}

public class ChannelCreationRequest
{
    /// <summary>
    /// Human-readable name for the channel
    /// </summary>
    public required string ChannelName { get; set; }

    /// <summary>
    /// Secret string provided by the user to verify their association with the clients. While a client registers, Wbskt will check for this secret in the client's request.
    /// This Secret must be provided in the client's config.
    /// </summary>
    public required string ChannelSecret { get; set; }

    /// <summary>
    /// Used by a publisher. This ID is required to figure out which all channels the trigger must go to.
    /// </summary>
    public Guid ChannelPublisherId { get; set; }

    /// <summary>
    /// Will be fetched from the token.
    /// </summary>
    [JsonIgnore]
    public int UserId { get; set; }
}
