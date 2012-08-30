IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FieldMappings]') AND type in (N'U'))
DROP TABLE [dbo].[FieldMappings]
GO

/****** Object:  Table [dbo].[SalsaDataTypes]    Script Date: 08/30/2012 15:05:08 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SalsaDataTypes]') AND type in (N'U'))
DROP TABLE [dbo].[SalsaDataTypes]
GO

/****** Object:  Table [dbo].[SalsaDataTypes]    Script Date: 08/30/2012 15:05:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[SalsaDataTypes](
	[DataType] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_SalsaDataTypes] PRIMARY KEY CLUSTERED 
(
	[DataType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


