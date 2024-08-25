/* ------------------------------------ */
/* Clients_GetBy_ChannelSubscriberId    */
/* Author:	Richard Joy                 */
/* Updated by: Richard Joy              */
/* Create date: 25-Aug-2024             */
/* Description: Self explanatory        */
/* ------------------------------------ */
CREATE PROCEDURE dbo.Clients_GetBy_ChannelSubscriberId
(
  @ChannelSubscriberId UNIQUEIDENTIFIER
)
AS
BEGIN
  SET NOCOUNT ON;

  SELECT Id
       , ClientName
       , ClientUniqueId
       , TokenId
       , Disabled
       , ChannelSubscriberId
    FROM dbo.Clients 
   WHERE ChannelSubscriberId = @ChannelSubscriberId
END;