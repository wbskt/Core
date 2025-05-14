/* -------------------------------- */
/* ClientConnections_GetBy_Id       */
/* Author: Richard Joy              */
/* Updated by: Richard Joy          */
/* Create date: 26-Aug-2024         */
/* Description: Self explanatory    */
/* -------------------------------- */
CREATE PROCEDURE dbo.ClientConnections_GetBy_Id
  @Id INT
AS
BEGIN
  SET NOCOUNT ON;

  SELECT Id
       , ClientName
       , ClientUniqueId
       , ServerId
       , UserId
    FROM dbo.ClientConnections
   WHERE Id = @Id
END;
