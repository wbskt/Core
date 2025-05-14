/* ---------------------------------------------------------------- */
/* ClientConnections_Upsert                                         */
/* Author: Richard Joy                                              */
/* Updated by: Richard Joy                                          */
/* Create date: 25-Apr-2025                                         */
/* Description: Insert or update a client based on ClientUniqueId   */
/* ---------------------------------------------------------------- */
CREATE PROCEDURE dbo.ClientConnectionsChannels_Add
    @ClientId   INT,
    @ChannelId  INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (
        SELECT 1 FROM dbo.ClientConnectionsChannels WHERE ClientId = @ClientId AND ChannelId = @ChannelId
    )
    BEGIN
        INSERT INTO dbo.ClientConnectionsChannels (ClientId, ChannelId)
        VALUES (@ClientId, @ChannelId);
    END
END