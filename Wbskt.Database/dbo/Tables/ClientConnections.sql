CREATE TABLE [dbo].[ClientConnections] (
    [Id]                    INT                 IDENTITY (1, 1) NOT NULL,
    [UserId]                INT                 NOT NULL,
    [ClientName]            VARCHAR (100)       NOT NULL,
    [ClientUniqueId]        UNIQUEIDENTIFIER    NOT NULL,
    CONSTRAINT [Pk_ClientConnections]       PRIMARY KEY CLUSTERED       ([Id]               ASC),
    CONSTRAINT [Unq_ClientUniqueId]         UNIQUE      NONCLUSTERED    ([ClientUniqueId]   ASC),
    CONSTRAINT [Unq_ClientName_UserId]      UNIQUE      NONCLUSTERED    ([ClientUniqueId]   ASC,    [UserId]    ASC),
    CONSTRAINT [Fk_ClientConnections_Users]  FOREIGN KEY                ([UserId])          REFERENCES  [dbo].[Users]   ([Id])
);
