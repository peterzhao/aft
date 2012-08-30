
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SyncConfigs]') AND type in (N'U'))
DROP TABLE [dbo].[SyncConfigs]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FieldMappings]') AND type in (N'U'))
DROP TABLE [dbo].[FieldMappings]
GO

/****** Object:  Table [dbo].[ObjectTypes]    Script Date: 08/30/2012 14:27:27 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ObjectTypes]') AND type in (N'U'))
DROP TABLE [dbo].[ObjectTypes]
GO

/****** Object:  Table [dbo].[ObjectTypes]    Script Date: 08/30/2012 14:27:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ObjectTypes](
	[ObjectType] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_ObjectTypes] PRIMARY KEY CLUSTERED 
(
	[ObjectType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


