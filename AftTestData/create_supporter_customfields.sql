SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Supporter_CustomFields]') AND type in (N'U'))
DROP TABLE [dbo].[Supporter_CustomFields]
GO

CREATE TABLE [dbo].[Supporter_CustomFields](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nchar](255) NOT NULL,
	[Type] [nchar](50) NOT NULL,
 CONSTRAINT [PK_Supporter_CustomFields] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Supporter_CustomField_Values]') AND type in (N'U'))
DROP TABLE [dbo].[Supporter_CustomField_Values]
GO

CREATE TABLE [dbo].[Supporter_CustomField_Values](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Supporter_Id] [int] NOT NULL,
	[Supporter_Custom_Field_Id] [int] NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Supporter_CustomField_Values] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO




