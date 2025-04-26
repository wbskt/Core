/* -------------------------------- */
/* Clients_FindBy_ClientUniqueId    */
/* Author: Richard Joy              */
/* Updated by: Richard Joy          */
/* Create date: 26-Apr-2025         */
/* Description: Self explanatory    */
/* -------------------------------- */
CREATE PROCEDURE dbo.Clients_FindBy_ClientUniqueId
(
  @ClientUniqueId UNIQUEIDENTIFIER
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id
    FROM dbo.Clients
    WHERE ClientUniqueId = @ClientUniqueId;
END;
