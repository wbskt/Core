CREATE TABLE [dbo].[SocketServers] (
    [Id]        INT           IDENTITY (1, 1) NOT NULL,
    [IPAddress] VARCHAR (100) NOT NULL,
    [Port]      INT           NOT NULL,
    CONSTRAINT [Pk_SocketServers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [Unq_SocketServers] UNIQUE NONCLUSTERED ([IPAddress] ASC, [Port] ASC)
);

