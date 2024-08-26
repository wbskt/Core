/* ------------------------------------ */
/* Channels_GetBy_ChannelPublisherId    */
/* Author: Richard Joy                  */
/* Updated by: Richard Joy              */
/* Create date: 25-Aug-2024             */
/* Description: Self explanatory        */
/* ------------------------------------ */
CREATE PROCEDURE dbo.Channels_GetBy_ChannelPublisherId
(
  @ChannelPublisherId UNIQUEIDENTIFIER
)
AS
BEGIN
  SET NOCOUNT ON;

  SELECT Id
       , ChannelName
       , ChannelPublisherId
       , ChannelSubscriberId
       , UserId
    FROM dbo.Channels 
   WHERE ChannelPublisherId = @ChannelPublisherId
END;