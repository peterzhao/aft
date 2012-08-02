USE [Aft]
GO

/****** Object:  Table [dbo].[Synchronizations]    Script Date: 08/01/2012 23:01:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.SyncRuns', 'U') IS NOT NULL  
DROP TABLE dbo.SyncRuns
GO

CREATE TABLE [dbo].[SyncRuns](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Complete] [bit] NOT NULL,
	[StartTime] [datetime2](7) NOT NULL,
	[CurrentRecord] [int] NOT NULL,
	[LastUpdatedMinimum] [datetime2](7) NULL
	PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

