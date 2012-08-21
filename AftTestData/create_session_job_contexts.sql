

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_SyncEvents_TimeStamp]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[SyncEvents] DROP CONSTRAINT [DF_SyncEvents_TimeStamp]
END

GO



/****** Object:  Table [dbo].[SyncEvents]    Script Date: 08/09/2012 22:39:55 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SyncEvents]') AND type in (N'U'))
DROP TABLE [dbo].[SyncEvents]
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[JobContexts]') AND type in (N'U'))
DROP TABLE [dbo].[JobContexts]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SessionContexts]') AND type in (N'U'))
DROP TABLE [dbo].[SessionContexts]
GO

CREATE TABLE [dbo].[SessionContexts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[State] [nvarchar](20) NULL,
	[StartTime] [datetime2](7) NULL,
	[FinishedTime] [datetime2](7) NULL,
	[MinimumModifiedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_SessionContexts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[JobContexts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CurrentRecord] [int] NOT NULL,
	[JobName] [nvarchar](max) NULL,
	[StartTime] [datetime2](7) NULL,
	[FinishedTime] [datetime2](7) NULL,
	[SessionContext_Id] [int] NOT NULL,
 CONSTRAINT [PK_JobContexts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[JobContexts]  WITH CHECK ADD FOREIGN KEY([SessionContext_Id])
REFERENCES [dbo].[SessionContexts] ([Id])
GO


CREATE TABLE [dbo].[SyncEvents](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ObjectId] [int] NOT NULL,
	[ExternalId] [int] NOT NULL,
	[ObjectType] [nvarchar](250) NOT NULL,
	[Destination] [nvarchar](250) NOT NULL,
	[EventType] [nvarchar](50) NOT NULL,
	[Error] [nvarchar](max) NULL,
	[Data] [nvarchar](max) NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[SessionContext_Id] [int] NOT NULL,
 CONSTRAINT [PK_SyncEvents] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SyncEvents] ADD  CONSTRAINT [DF_SyncEvents_TimeStamp]  DEFAULT (getdate()) FOR [TimeStamp]
GO

ALTER TABLE [dbo].[SyncEvents]  WITH CHECK ADD FOREIGN KEY([SessionContext_Id])
REFERENCES [dbo].[SessionContexts] ([Id])
GO


