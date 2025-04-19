/* ------------------------------------ */
/* Channels_GetBy_ChannelSubscriberId   */
/* Author: Richard Joy                  */
/* Updated by: Richard Joy              */
/* Create date: 19-Apr-2025             */
/* Description: Self explanatory        */
/* ------------------------------------ */
CREATE PROCEDURE dbo.Channels_GetBy_ChannelSubscriberId
(
    @ChannelSubscriberId UNIQUEIDENTIFIER
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id
         , ChannelName
         , ChannelPublisherId
         , ChannelSubscriberId
         , UserId
         , ServerId
         , RetentionTime
         , ChannelSecret
    FROM dbo.Channels
    WHERE ChannelSubscriberId = @ChannelSubscriberId
END;
