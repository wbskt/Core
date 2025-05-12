/* ---------------------------------------------------- */
/* Clients_Exists_ClientName_ChannelSubscriptionId      */
/* Author: Richard Joy                                  */
/* Updated by: Richard Joy                              */
/* Create date: 12-May-2025                             */
/* Description: Self explanatory                        */
/* ---------------------------------------------------- */
CREATE PROCEDURE dbo.Clients_Exists_ClientName_ChannelSubscriptionId
(
    @ChannelSubscriberId    UNIQUEIDENTIFIER,
    @ClientName             VARCHAR (100)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id
    FROM dbo.Clients
    WHERE ChannelSubscriberId = @ChannelSubscriberId AND ClientName = @ClientName;
END;
