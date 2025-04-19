/* -------------------------------- */
/* Channels_ServerId_UpdateBy_Id    */
/* Author: Richard Joy              */
/* Updated by: Richard Joy          */
/* Create date: 20-Apr-2024         */
/* Description: Self explanatory    */
/* -------------------------------- */
CREATE PROCEDURE dbo.Channels_ServerId_UpdateBy_Id
(
  @Id       INT,
  @ServerId INT
)
AS
BEGIN
  SET NOCOUNT ON;

    UPDATE dbo.Channels
    SET ServerId = @ServerId
    WHERE Id = @Id
END;
