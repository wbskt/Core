/* -------------------------------- */
/* Servers_UpdatePublicDomainName   */
/* Author: Richard Joy              */
/* Updated by: Richard Joy          */
/* Create date: 30-Apr-2025         */
/* Description: Self explanatory    */
/* -------------------------------- */
CREATE PROCEDURE dbo.Servers_UpdatePublicDomainName
(
  @Id               INT,
  @PublicDomainName VARCHAR(100)
)
AS
BEGIN
  SET NOCOUNT ON;

    UPDATE dbo.Servers
    SET PublicDomainName = @PublicDomainName
    WHERE Id = @Id
END;
