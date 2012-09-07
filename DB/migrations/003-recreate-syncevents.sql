Drop table SyncEvents

CREATE TABLE [SyncEvents] (
    [Id] [int] NOT NULL IDENTITY,
    [SalsaKey] [int] NOT NULL,
    [ObjectType] [nvarchar](max),
    [Data] [nvarchar](max),
    [EventType] [nvarchar](max),
    [TimeStamp] [datetime] NOT NULL DEFAULT (getdate()),
    [Error] [nvarchar](max),
    [Destination] [nvarchar](max),
    [SessionContext_Id] [int],
    CONSTRAINT [PK_SyncEvents] PRIMARY KEY ([Id])
)

