/* -------------------------------- */
/* Channels_GetBy_SocketServerId    */
/* Author: Richard Joy              */
/* Updated by: Richard Joy          */
/* Create date: 24-Aug-2024         */
/* Description: Self explanatory    */
/* -------------------------------- */
CREATE PROCEDURE dbo.Channels_GetBy_SocketServerId
(
  @SocketServerId INT
)
AS
BEGIN
  SET NOCOUNT ON;

  SELECT Id
       , ChannelName
       , ChannelPublisherId
       , ChannelSubscriberId
       , UserId
       , RetentionTime
       , SocketServerId
    FROM dbo.Channels 
   WHERE SocketServerId = @SocketServerId
END;