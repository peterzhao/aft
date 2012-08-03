USE [AFT]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__JobContex__Sessi__286302EC]') AND parent_object_id = OBJECT_ID(N'[dbo].[JobContexts]'))
ALTER TABLE [dbo].[JobContexts] DROP CONSTRAINT [FK__JobContex__Sessi__286302EC]
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


