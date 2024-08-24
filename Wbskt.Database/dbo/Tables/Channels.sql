/* -------------------------------- */
/* Author:	Richard Joy             */
/* Updated by: Richard Joy          */
/* Create date: 24-Aug-2024         */
/* Description: Self explanatory    */
/* -------------------------------- */
CREATE TABLE [dbo].[Channels] (
    [Id]          INT              IDENTITY (1, 1)  NOT NULL,
    [ChannelName] VARCHAR (100)                     NOT NULL,
    [UserId]      INT                               NOT NULL,
    [ChannelKey]  UNIQUEIDENTIFIER                  NOT NULL,
    CONSTRAINT [pk_Channels]        PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [fk_Channels_Users]  FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);

