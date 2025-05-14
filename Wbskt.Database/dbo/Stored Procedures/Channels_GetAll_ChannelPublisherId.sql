/* ------------------------------------ */
/* Channels_GetAll_ChannelPublisherId   */
/* Author: Richard Joy                  */
/* Updated by: Richard Joy              */
/* Create date: 25-Aug-2024             */
/* Description: Self explanatory        */
/* ------------------------------------ */
CREATE PROCEDURE dbo.Channels_GetAll_ChannelPublisherId
    @ChannelPublisherId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  Id,
            ChannelName,
            ChannelPublisherId,
            ChannelSubscriberId,
            UserId,
            ChannelSecret
    FROM    dbo.Channels
    WHERE   ChannelPublisherId = @ChannelPublisherId
END;
