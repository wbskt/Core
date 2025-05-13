﻿using System.ComponentModel.DataAnnotations;

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

    public static InvalidOperationException ChannelIdNotExists(int channelId)
    {
        return new InvalidOperationException($"channel with id: '{channelId}' does not exists");
    }

    public static InvalidOperationException InvalidId(int id, string resource)
    {
        return new InvalidOperationException($"invalid id provided: '{id}' for resource: {resource}");
    }

    public static ValidationException ChannelSubscriberIdNotExists(Guid channelSubscriberId)
    {
        return new ValidationException($"channel with subscriberId: '{channelSubscriberId}' does not exists");
    }

    public static InvalidOperationException UnknownSocketServer(int id)
    {
        return new InvalidOperationException($"the socket server with id: '{id}' is not present in the registered servers");
    }

    public static ValidationException ClientWithSameNameExists(string reqClientName)
    {
        return new ValidationException($"client with name: '{reqClientName}' already exists in the same channel");
    }

    public static UnauthorizedAccessException UnauthorizedAccessToChannels()
    {
        return new UnauthorizedAccessException("client does not have permission to subscribe to all of the requested channels");
    }

    public static InvalidOperationException SocketServerUnavailable()
    {
        return new InvalidOperationException($"the are no socket servers registered");
    }
}
