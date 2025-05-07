using System.ComponentModel.DataAnnotations;

namespace Wbskt.Common.Exceptions;

public static class WbsktExceptions
{
    public static UnauthorizedAccessException ThrowEmailIdExists(string emailId)
    {
        return new UnauthorizedAccessException($"emailId: '{emailId}' already exists");
    }
    public static ValidationException ThrowChannelExists(string channelName)
    {
        return new ValidationException($"channel: '{channelName}' already exists");
    }
    public static ValidationException ThrowChannelSubscriberIdNotExists(Guid channelSubscriberId)
    {
        return new ValidationException($"channel with subscriberId: '{channelSubscriberId}' does not exists");
    }
}
