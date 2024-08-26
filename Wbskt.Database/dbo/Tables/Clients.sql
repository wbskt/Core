CREATE TABLE [dbo].[Clients] (
    [Id]                  INT              IDENTITY (1, 1) NOT NULL,
    [ClientName]          VARCHAR (100)    NOT NULL,
    [ClientUniqueId]      VARCHAR (100)    NOT NULL,
    [ChannelSubscriberId] UNIQUEIDENTIFIER NOT NULL,
    [Disabled]            BIT              NOT NULL,
    [TokenId]             UNIQUEIDENTIFIER DEFAULT (CONVERT([uniqueidentifier],CONVERT([binary],(0)))) NOT NULL,
    CONSTRAINT [Pk_Clients] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [Fk_Clients] FOREIGN KEY ([ChannelSubscriberId]) REFERENCES [dbo].[Channels] ([ChannelSubscriberId]),
    CONSTRAINT [Unq_Clients] UNIQUE NONCLUSTERED ([ClientUniqueId] ASC, [ChannelSubscriberId] ASC)
);

