/* -------------------------------- */
/* ClientConnections_GetAll         */
/* Author: Richard Joy              */
/* Updated by: Richard Joy          */
/* Create date: 25-Aug-2024         */
/* Description: Self explanatory    */
/* -------------------------------- */
CREATE PROCEDURE dbo.ClientConnections_GetAll
AS
BEGIN
  SET NOCOUNT ON;

  SELECT Id
       , ClientName
       , ClientUniqueId
       , ServerId
       , UserId
    FROM dbo.ClientConnections
END;
