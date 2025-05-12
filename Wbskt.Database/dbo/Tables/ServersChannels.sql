CREATE TABLE [dbo].[ServersChannels] (
    [ServerId]      INT     NOT NULL,
    [ChannelId]     INT     NOT NULL,
    CONSTRAINT  [Unq_ServersChannels]   UNIQUE NONCLUSTERED ([ServerId] ASC, [ChannelId] ASC),
    CONSTRAINT  [Fk_ServersChannels_Servers]     FOREIGN KEY     ([ServerId])   REFERENCES [dbo].[Servers]   ([Id]),
    CONSTRAINT  [Fk_ServersChannels_Channels]    FOREIGN KEY     ([ChannelId])  REFERENCES [dbo].[Channels]  ([Id])
);
