/* ---------------------------------------------------------------- */
/* Clients_Upsert                                                   */
/* Author: Richard Joy                                              */
/* Updated by: Richard Joy                                          */
/* Create date: 25-Aug-2024                                         */
/* Description: Insert or update a client based on ClientUniqueId   */
/* ---------------------------------------------------------------- */
CREATE PROCEDURE dbo.Clients_Upsert
(
    @Id                     INT                 OUTPUT,
    @TokenId                UNIQUEIDENTIFIER,
    @Token                  VARCHAR(512),
    @ClientName             VARCHAR(100),
    @ClientUniqueId         UNIQUEIDENTIFIER,
    @ChannelSubscriberId    UNIQUEIDENTIFIER,
    @Disabled               BIT                 = 0
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ExistingId INT;

    SELECT @ExistingId = Id
    FROM dbo.Clients
    WHERE ClientUniqueId = @ClientUniqueId;

    IF @ExistingId IS NOT NULL
        BEGIN
            -- Update existing client
            UPDATE dbo.Clients
            SET TokenId             = @TokenId,
                Token               = @Token,
                ClientName          = @ClientName,
                ChannelSubscriberId = @ChannelSubscriberId,
                Disabled            = @Disabled
            WHERE
                Id                  = @ExistingId;

            SET @Id = @ExistingId;
        END
    ELSE
        BEGIN
            -- Insert new client
            INSERT INTO dbo.Clients
            ( TokenId
            , Token
            , ClientName
            , ClientUniqueId
            , ChannelSubscriberId
            , Disabled)
            VALUES
                (@TokenId
                , @Token
                , @ClientName
                , @ClientUniqueId
                , @ChannelSubscriberId
                , @Disabled);

            SET @Id = SCOPE_IDENTITY();
        END
END;
