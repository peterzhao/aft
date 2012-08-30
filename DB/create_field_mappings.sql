/****** Object:  Table [dbo].[FieldMappings]    Script Date: 08/29/2012 13:29:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
/****** Object:  Table [dbo].[FieldMappings]    Script Date: 08/29/2012 13:29:54 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FieldMappings]') AND type in (N'U'))
DROP TABLE [dbo].[FieldMappings]
GO




CREATE TABLE [dbo].[FieldMappings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ObjectType] [nvarchar](200) NOT NULL,
	[SalsaField] [nvarchar](200) NOT NULL,
	[AftField] [nvarchar](200) NOT NULL,
	[DataType] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_FieldMappings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


