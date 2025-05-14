CREATE PROCEDURE dbo.ClientConnectionsChannels_SetForClient
    @ClientId INT,
    @ChannelIds dbo.IdListTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    -- Remove old relations not in the new list
    DELETE FROM dbo.ClientConnectionsChannels
    WHERE ClientId = @ClientId
      AND ChannelId NOT IN (SELECT Id FROM @ChannelIds);

    -- Insert new relations
    INSERT INTO dbo.ClientConnectionsChannels (ClientId, ChannelId)
    SELECT @ClientId, Id
    FROM @ChannelIds
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.ClientConnectionsChannels
        WHERE ClientId = @ClientId AND ChannelId = Id
    );
END