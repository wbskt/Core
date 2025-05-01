/* --------------------------------------------- */
/* Channels_ServerId_UpdateMultiple              */
/* Author: Richard Joy                           */
/* Updated by: Richard Joy                       */
/* Create date: 1-May-2025                       */
/* Description: Updates ServerId for multiple Id */
/* --------------------------------------------- */
CREATE PROCEDURE dbo.Channels_ServerId_UpdateMultiple
(
    @Updates dbo.IdIntValueTableType READONLY
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE  C
    SET     C.ServerId = U.IntValue
    FROM    dbo.Channels C
    INNER JOIN @Updates U ON C.Id = U.Id;
END;
