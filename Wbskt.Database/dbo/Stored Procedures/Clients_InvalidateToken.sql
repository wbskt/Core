/* -------------------------------- */
/* Clients_InvalidateToken          */
/* Author: Richard Joy              */
/* Updated by: Richard Joy          */
/* Create date: 26-Aug-2024         */
/* Description: Self explanatory    */
/* -------------------------------- */
CREATE PROCEDURE dbo.Clients_InvalidateToken
(
  @Id INT
)
AS
BEGIN
  SET NOCOUNT ON;

    UPDATE dbo.Clients
    SET TokenId = CAST( CAST(0 AS BINARY) AS UNIQUEIDENTIFIER),
        Token = ''
    WHERE Id = @Id
END;
