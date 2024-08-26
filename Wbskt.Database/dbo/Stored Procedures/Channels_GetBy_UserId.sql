/* -------------------------------- */
/* Channels_GetBy_UserId            */
/* Author: Richard Joy              */
/* Updated by: Richard Joy          */
/* Create date: 24-Aug-2024         */
/* Description: Self explanatory    */
/* -------------------------------- */
CREATE PROCEDURE dbo.Channels_GetBy_UserId
(
  @UserId INT
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
       , ServerId
    FROM dbo.Channels 
   WHERE UserId = @UserId
END;