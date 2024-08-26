/* -------------------------------- */
/* User_Insert                      */
/* Author: Richard Joy              */
/* Updated by: Richard Joy          */
/* Create date: 24-Aug-2024         */
/* Description: Self explanatory    */
/* -------------------------------- */
CREATE PROCEDURE dbo.User_Insert
(
  @Id			INT OUTPUT
, @UserName		VARCHAR(100)
, @EmailId		VARCHAR(100)
, @PasswordHash VARCHAR(512)
, @PasswordSalt VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Users
    ( UserName
    , EmailId
    , PasswordHash
    , PasswordSalt
    )
    VALUES
        ( @UserName
        , @EmailId
        , @PasswordHash
        , @PasswordSalt
        );
    SELECT @Id = SCOPE_IDENTITY();
END;