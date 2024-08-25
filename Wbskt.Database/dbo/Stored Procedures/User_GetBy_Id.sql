/* -------------------------------- */
/* User_GetBy_Id                    */
/* Author:	Richard Joy             */
/* Updated by: Richard Joy          */
/* Create date: 24-Aug-2024         */
/* Description: Self explanatory    */
/* -------------------------------- */
CREATE PROCEDURE dbo.User_GetBy_Id
(
  @Id INT
)
AS
BEGIN
  SET NOCOUNT ON;

  SELECT Id
       , UserName
       , EmailId
       , PasswordHash
       , PasswordSalt
    FROM dbo.Users 
   WHERE Id = @Id
END;