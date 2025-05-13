/* -------------------------------- */
/* Channels_GetAll_ServerId          */
/* Author: Richard Joy              */
/* Updated by: Richard Joy          */
/* Create date: 26-Aug-2024         */
/* Description: Self explanatory    */
/* -------------------------------- */
CREATE PROCEDURE dbo.Channels_GetAll_ServerId
(
  @ServerId INT
)
AS
BEGIN
  SET NOCOUNT ON;

  SELECT Id
       , ChannelName
       , ChannelPublisherId
       , ChannelSubscriberId
       , UserId
       , ChannelSecret
    FROM dbo.Channels C INNER JOIN dbo.ServersChannels SC on C.Id = SC.ChannelId
   WHERE SC.ServerId = @ServerId
END;
