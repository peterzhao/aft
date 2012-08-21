SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SupporterCustomFieldValues]') AND type in (N'U'))
DROP TABLE [dbo].[SupporterCustomFieldValues]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SupporterCustomFields]') AND type in (N'U'))
DROP TABLE [dbo].[SupporterCustomFields]
GO

CREATE TABLE [dbo].[SupporterCustomFields](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_SupporterCustomFields] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[SupporterCustomFieldValues](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Supporter_Id] [int] NOT NULL,
	[SupporterCustomField_Id] [int] NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_SupporterCustomFieldValues] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SupporterCustomFieldValues]  WITH CHECK ADD FOREIGN KEY([SupporterCustomField_Id])
REFERENCES [dbo].[SupporterCustomFields] ([Id])
GO


ALTER TABLE [dbo].[SupporterCustomFieldValues]  WITH CHECK ADD FOREIGN KEY([Supporter_Id])
REFERENCES [dbo].[Supporters] ([Id])
GO




