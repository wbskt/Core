/* -------------------------------- */
/* Servers_UpdateStatus             */
/* Author: Richard Joy              */
/* Updated by: Richard Joy          */
/* Create date: 26-Aug-2024         */
/* Description: Self explanatory    */
/* -------------------------------- */
CREATE PROCEDURE dbo.Servers_UpdateStatus
(
  @Id     INT,
  @Active BIT
)
AS
BEGIN
  SET NOCOUNT ON;

    UPDATE dbo.Servers
    SET Active = @Active
    WHERE Id = @Id
END;