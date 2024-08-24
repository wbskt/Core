CREATE TABLE [dbo].[Channels] (
    [Id]                    INT                 IDENTITY (1, 1) NOT NULL,
    [ChannelName]           VARCHAR (100)       NOT NULL,
    [UserId]                INT                 NOT NULL,
    [ChannelSubscriberId]   UNIQUEIDENTIFIER    NOT NULL,
    [ChannelPublisherId]    UNIQUEIDENTIFIER    NOT NULL,
    [RetentionTime]         INT                 NOT NULL        DEFAULT 0, 
    CONSTRAINT [Pk_Channels]        PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [Unq_Channels_UserId_ChannelName] UNIQUE NONCLUSTERED ([UserId], [ChannelName]),
    CONSTRAINT [Fk_Channels_Users]  FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])

);

