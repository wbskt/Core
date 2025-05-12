/* ---------------------------------- */
/* ClientConnections_GetAll_ChannelId */
/* Author: Richard Joy                */
/* Updated by: Richard Joy            */
/* Create date: 25-Aug-2024           */
/* Description: Self explanatory      */
/* ---------------------------------- */
CREATE PROCEDURE dbo.ClientConnections_GetAll_ChannelId
(
  @ChannelId INT
)
AS
BEGIN
  SET NOCOUNT ON;

  SELECT Id
       , ClientName
       , ClientUniqueId
       , UserId
    FROM dbo.ClientConnections C INNER JOIN ClientConnectionsChannels CCC ON C.Id = CCC.ChannelId
   WHERE CCC.ChannelId = @ChannelId
END;
