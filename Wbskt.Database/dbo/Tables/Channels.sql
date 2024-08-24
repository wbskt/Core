CREATE TABLE [dbo].[Channels] (
    [Id]          INT              IDENTITY (1, 1) NOT NULL,
    [ChannelName] VARCHAR (100)    NOT NULL,
    [UserId]      INT              NOT NULL,
    [ChannelKey]  UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [Pk_Channels] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [Fk_Channels_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);

