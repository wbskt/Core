/* -------------------------------- */
/* Clients_Insert                   */
/* Author:	Richard Joy             */
/* Updated by: Richard Joy          */
/* Create date: 25-Aug-2024         */
/* Description: Self explanatory    */
/* -------------------------------- */
CREATE PROCEDURE dbo.Clients_Insert
(
  @Id			        INT OUTPUT
, @TokenId              UNIQUEIDENTIFIER
, @ClientName           VARCHAR(100)
, @ClientUniqueId       VARCHAR(100)
, @ChannelSubscriberId  UNIQUEIDENTIFIER
, @Disabled             BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Clients
    ( TokenId
    , ClientName
    , ClientUniqueId
    , ChannelSubscriberId
    , Disabled
    )
    VALUES
        ( @TokenId
        , @ClientName
        , @ClientUniqueId
        , @ChannelSubscriberId
        , @Disabled
        );
    SELECT @Id = SCOPE_IDENTITY();
END;