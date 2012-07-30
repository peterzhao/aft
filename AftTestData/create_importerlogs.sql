USE [AFT]
GO

/****** Object:  Table [dbo].[ImporterLogs]    Script Date: 07/30/2012 14:29:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ImporterLogs](
	[time_stamp] [datetime] NULL,
	[level] [nchar](20) NULL,
	[threadid] [int] NULL,
	[message] [nvarchar](max) NULL
) ON [PRIMARY]

GO


