/* -------------------------------- */
/* Channels_GetBy_ChannelKey        */
/* Author:	Richard Joy             */
/* Updated by: Richard Joy          */
/* Create date: 24-Aug-2024         */
/* Description: Self explanatory    */
/* -------------------------------- */
CREATE PROCEDURE dbo.Channels_GetBy_ChannelKey
(
  @ChannelKey UNIQUEIDENTIFIER
)
AS
BEGIN
  SET NOCOUNT ON;

  SELECT Id
       , ChannelName
       , ChannelKey
       , UserId
    FROM dbo.Channels 
   WHERE ChannelKey = @ChannelKey
END;