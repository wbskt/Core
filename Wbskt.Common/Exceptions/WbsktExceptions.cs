using System.ComponentModel.DataAnnotations;

namespace Wbskt.Common.Exceptions;

public static class WbsktExceptions
{
    public static UnauthorizedAccessException EmailIdExists(string emailId)
    {
        return new UnauthorizedAccessException($"emailId: '{emailId}' already exists");
    }
    public static ValidationException ChannelExists(string channelName)
    {
        return new ValidationException($"channel: '{channelName}' already exists");
    }
    public static ValidationException ChannelSubscriberIdNotExists(Guid channelSubscriberId)
    {
        return new ValidationException($"channel with subscriberId: '{channelSubscriberId}' does not exists");
    }

    public static InvalidOperationException UnknownSocketServer(int id)
    {
        return new InvalidOperationException($"the socket server with id: '{id}' is not present in the registered servers");
    }
}
