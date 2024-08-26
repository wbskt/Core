/* -------------------------------- */
/* Servers_GetAll                   */
/* Author: Richard Joy              */
/* Updated by: Richard Joy          */
/* Create date: 26-Aug-2024         */
/* Description: Self explanatory    */
/* -------------------------------- */
CREATE PROCEDURE dbo.Servers_GetAll
AS
BEGIN
  SET NOCOUNT ON;

  SELECT Id
       , IPAddress
       , Port
       , Active
    FROM dbo.Servers 
END;