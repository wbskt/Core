/* -------------------------------------------- */
/* ClientConnections_FindBy_ClientName_UserId   */
/* Author: Richard Joy                          */
/* Updated by: Richard Joy                      */
/* Create date: 12-May-2025                     */
/* Description: Self explanatory                */
/* -------------------------------------------- */
CREATE PROCEDURE dbo.ClientConnections_FindBy_ClientName_UserId
(
    @UserId         INT,
    @ClientName     VARCHAR (100)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id
    FROM dbo.ClientConnections
    WHERE UserId = @UserId AND ClientName = @ClientName;
END;
