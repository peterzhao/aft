
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SyncConfigs]') AND type in (N'U'))
DROP TABLE [dbo].[SyncConfigs]
GO


/****** Object:  Table [dbo].[SyncDirections]    Script Date: 08/30/2012 14:27:27 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SyncDirections]') AND type in (N'U'))
DROP TABLE [dbo].[SyncDirections]
GO

/****** Object:  Table [dbo].[SyncDirections]    Script Date: 08/30/2012 14:27:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SyncDirections](
	[SyncDirection] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_SyncDirections] PRIMARY KEY CLUSTERED 
(
	[SyncDirection] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


