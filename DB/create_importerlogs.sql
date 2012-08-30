
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ImporterLogs]') AND type in (N'U'))
DROP TABLE [dbo].[ImporterLogs]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ImporterLogs](
	[time_stamp] [datetime] NULL,
	[level] [nchar](20) NULL,
	[threadid] [int] NULL,
	[message] [nvarchar](max) NULL,
	[exception] [nvarchar](max) NULL
) ON [PRIMARY]

GO


