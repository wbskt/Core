CREATE PROCEDURE dbo.ClientConnectionsChannels_Remove
    @ClientId INT,
    @ChannelId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.ClientConnectionsChannels
    WHERE ClientId = @ClientId AND ChannelId = @ChannelId;
END