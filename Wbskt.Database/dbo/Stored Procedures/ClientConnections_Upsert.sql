/* ---------------------------------------------------------------- */
/* ClientConnections_Upsert                                         */
/* Author: Richard Joy                                              */
/* Updated by: Richard Joy                                          */
/* Create date: 25-Apr-2025                                         */
/* Description: Insert or update a client based on ClientUniqueId   */
/* ---------------------------------------------------------------- */
CREATE PROCEDURE dbo.ClientConnections_Upsert
    @Id                     INT                 OUTPUT,
    @ClientName             VARCHAR(100),
    @ClientUniqueId         UNIQUEIDENTIFIER,
    @UserId                 INT,
    @ServerId               INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ExistingId INT;

    SELECT @ExistingId = Id
    FROM dbo.ClientConnections
    WHERE ClientUniqueId = @ClientUniqueId;

    IF @ExistingId IS NOT NULL
        BEGIN
            -- Update existing client; only name can be updated
            UPDATE dbo.ClientConnections
            SET ClientName          = @ClientName,
                ServerId            = @ServerId
            WHERE
                Id                  = @ExistingId;

            SET @Id = @ExistingId;
        END
    ELSE
        BEGIN
            -- Insert new client
            INSERT INTO dbo.ClientConnections
            ( ClientName
            , ServerId
            , ClientUniqueId
            , UserId)
            VALUES
                (@ClientName
                , @ServerId
                , @ClientUniqueId
                , @UserId);

            SET @Id = SCOPE_IDENTITY();
        END
END;
