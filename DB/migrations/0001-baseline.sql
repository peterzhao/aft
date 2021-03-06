
CREATE TABLE [dbo].[ImporterLogs](
	[time_stamp] [datetime] NOT NULL,
	[level] [nvarchar](50) NOT NULL,
	[threadid] [int] NOT NULL,
	[message] [nvarchar](max) NOT NULL,
	[exception] [nvarchar](max) NULL
) ON [PRIMARY]

CREATE TABLE [SessionContexts] (
    [Id] [int] NOT NULL IDENTITY,
    [State] [nvarchar](max),
    [StartTime] [datetime],
    [FinishedTime] [datetime],
    [MinimumModifiedDate] [datetime] NOT NULL,
    CONSTRAINT [PK_SessionContexts] PRIMARY KEY ([Id])
)
CREATE TABLE [JobContexts] (
    [Id] [int] NOT NULL IDENTITY,
    [JobName] [nvarchar](max),
    [StartTime] [datetime],
    [FinishedTime] [datetime],
    [CurrentRecord] [int] NOT NULL,
    [SessionContext_Id] [int],
    CONSTRAINT [PK_JobContexts] PRIMARY KEY ([Id])
)
CREATE INDEX [IX_SessionContext_Id] ON [JobContexts]([SessionContext_Id])
CREATE TABLE [SyncEvents] (
    [Id] [int] NOT NULL IDENTITY,
    [ObjectId] [int] NOT NULL,
    [ObjectType] [nvarchar](max),
    [Data] [nvarchar](max),
    [EventType] [nvarchar](max),
    [TimeStamp] [datetime] NOT NULL DEFAULT (getdate()),
    [Error] [nvarchar](max),
    [Destination] [nvarchar](max),
    [SessionContext_Id] [int],
    CONSTRAINT [PK_SyncEvents] PRIMARY KEY ([Id])
)
CREATE INDEX [IX_SessionContext_Id] ON [SyncEvents]([SessionContext_Id])
CREATE TABLE [FieldMappings] (
    [Id] [int] NOT NULL IDENTITY,
    [ObjectType] [nvarchar](100) not null,
    [SalsaField] [nvarchar](max) not null,
    [AftField] [nvarchar](max) not null,
    [DataType] [nvarchar](100) not null,
     [MappingRule] [nvarchar](100) not null,
    CONSTRAINT [PK_FieldMappings] PRIMARY KEY ([Id])
)
CREATE TABLE [SyncConfigs] (
    [Id] [int] NOT NULL IDENTITY,
    [ObjectType] [nvarchar](100) not null,
    [SyncDirection] [nvarchar](100) not null,
    [Order] [int] NOT NULL,
    CONSTRAINT [PK_SyncConfigs] PRIMARY KEY ([Id])
)

CREATE TABLE [dbo].[ObjectTypes]([ObjectType] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_ObjectTypes] PRIMARY KEY CLUSTERED 
(
	[ObjectType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[SalsaDataTypes](
	[DataType] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_SalsaDataTypes] PRIMARY KEY CLUSTERED 
(
	[DataType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[MappingRules](
	[MappingRule] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_MappingRules] PRIMARY KEY CLUSTERED 
(
	[MappingRule] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[SyncDirections](
	[SyncDirection] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_SyncDirections] PRIMARY KEY CLUSTERED 
(
	[SyncDirection] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]



ALTER TABLE [JobContexts] ADD CONSTRAINT [FK_JobContexts_SessionContexts_SessionContext_Id] FOREIGN KEY ([SessionContext_Id]) REFERENCES [SessionContexts] ([Id])
ALTER TABLE [SyncEvents] ADD CONSTRAINT [FK_SyncEvents_SessionContexts_SessionContext_Id] FOREIGN KEY ([SessionContext_Id]) REFERENCES [SessionContexts] ([Id])
ALTER TABLE [SyncConfigs] ADD CONSTRAINT [FK_SyncConfigs_ObjectTypes_ObjectType] FOREIGN KEY ([ObjectType]) REFERENCES [ObjectTypes] ([ObjectType])
ALTER TABLE [SyncConfigs] ADD CONSTRAINT [FK_SyncConfigs_SyncDirections_SyncDirection] FOREIGN KEY ([SyncDirection]) REFERENCES [SyncDirections] ([SyncDirection])
ALTER TABLE [FieldMappings] ADD CONSTRAINT [FK_FieldMappings_ObjectTypes_ObjectType] FOREIGN KEY ([ObjectType]) REFERENCES [ObjectTypes] ([ObjectType])
ALTER TABLE [FieldMappings] ADD CONSTRAINT [FK_FieldMappings_SalsaDataTypes_DataType] FOREIGN KEY ([DataType]) REFERENCES [SalsaDataTypes] ([DataType])
ALTER TABLE [FieldMappings] ADD CONSTRAINT [FK_FieldMappings_MappingRules_MappingRule] FOREIGN KEY ([MappingRule]) REFERENCES [MappingRules] ([MappingRule])

Insert ObjectTypes (ObjectType) values('supporter')
Insert ObjectTypes (ObjectType) values('groups')
Insert ObjectTypes (ObjectType) values('supporter_groups')
Insert ObjectTypes (ObjectType) values('event')
Insert ObjectTypes (ObjectType) values('donation')
Insert ObjectTypes (ObjectType) values('chapter')


Insert into SalsaDatatypes(DataType) values('string')
insert into SalsaDatatypes(DataType) values('int')
insert into SalsaDatatypes(DataType) values('bool')
insert into SalsaDatatypes(DataType) values('datetime')

Insert into  syncdirections(SyncDirection) values('export')
Insert into  syncdirections(SyncDirection) values('import')


Insert into  MappingRules(MappingRule) values('onlyIfBlank')
Insert into  MappingRules(MappingRule) values('readOnly')
Insert into  MappingRules(MappingRule) values('aftWins')
Insert into  MappingRules(MappingRule) values('salsaWins')
--//@UNDO




DROP INDEX [IX_SessionContext_Id] ON [SyncEvents]
DROP INDEX [IX_SessionContext_Id] ON [JobContexts]
ALTER TABLE [SyncEvents] DROP CONSTRAINT [FK_SyncEvents_SessionContexts_SessionContext_Id]
ALTER TABLE [JobContexts] DROP CONSTRAINT [FK_JobContexts_SessionContexts_SessionContext_Id]
ALTER TABLE [SyncConfigs] DROP CONSTRAINT [FK_SyncConfigs_ObjectTypes_ObjectType]
ALTER TABLE [SyncConfigs] DROP CONSTRAINT [FK_SyncConfigs_SyncDirections_SyncDirection]
ALTER TABLE [FieldMappings] DROP CONSTRAINT [FK_FieldMappings_ObjectTypes_ObjectType]
ALTER TABLE [FieldMappings] DROP CONSTRAINT [FK_FieldMappings_SalsaDataTypes_DataType]
ALTER TABLE [FieldMappings] DROP CONSTRAINT [FK_FieldMappings_MappingRules_MappingRule]


DROP TABLE [SyncConfigs]
DROP TABLE [FieldMappings]
DROP TABLE [SyncEvents]
DROP TABLE [JobContexts]
DROP TABLE [SessionContexts]


DROP TABLE [SalsaDataTypes]
DROP TABLE [ObjectTypes]
DROP TABLE [SyncDirections]
DROP TABLE [MappingRules]


DROP TABLE [ImporterLogs]