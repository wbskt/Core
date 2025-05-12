/* ------------------------------------ */
/* ClientConnections_GetAll_UserId      */
/* Author: Richard Joy                  */
/* Updated by: Richard Joy              */
/* Create date: 25-Aug-2024             */
/* Description: Self explanatory        */
/* ------------------------------------ */
CREATE PROCEDURE dbo.ClientConnections_GetAll_UserId
(
  @UserId INT
)
AS
BEGIN
  SET NOCOUNT ON;

  SELECT Id
       , ClientName
       , ClientUniqueId
       , UserId
    FROM dbo.ClientConnections
   WHERE UserId = @UserId
END;
