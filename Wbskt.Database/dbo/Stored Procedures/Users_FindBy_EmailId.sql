/* -------------------------------- */
/* Users_FindBy_EmailId             */
/* Author: Richard Joy              */
/* Updated by: Richard Joy          */
/* Create date: 26-Apr-2025         */
/* Description: Self explanatory    */
/* -------------------------------- */
CREATE PROCEDURE dbo.Users_FindBy_EmailId
(
    @EmailId UNIQUEIDENTIFIER
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id
    FROM dbo.Users
    WHERE EmailId = @EmailId;
END;
