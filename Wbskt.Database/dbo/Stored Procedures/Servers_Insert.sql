/* -------------------------------- */
/* Servers_Insert                   */
/* Author: Richard Joy              */
/* Updated by: Richard Joy          */
/* Create date: 26-Aug-2024         */
/* Description: Self explanatory    */
/* -------------------------------- */
CREATE PROCEDURE dbo.Servers_Insert
(
  @Id			        INT OUTPUT
, @IPAddress	        VARCHAR(100)
, @Port		            INT
, @Active               BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Servers
    ( IPAddress
    , Port
    , Active
    )
    VALUES
        ( @IPAddress
        , @Port
        , @Active
        );
    SELECT @Id = SCOPE_IDENTITY();
END;