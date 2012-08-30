/****** Object:  Table [dbo].[SyncConfigs]    Script Date: 08/29/2012 13:29:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
/****** Object:  Table [dbo].[SyncConfigs]    Script Date: 08/29/2012 13:29:54 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SyncConfigs]') AND type in (N'U'))
DROP TABLE [dbo].[SyncConfigs]
GO




CREATE TABLE [dbo].[SyncConfigs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ObjectType] [nvarchar](200) NOT NULL,
	[SyncDirection] [nvarchar](200) NOT NULL,
	[Order] int NOT NULL
 CONSTRAINT [PK_SyncConfigs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE dbo.SyncConfigs ADD CONSTRAINT
	FK_SyncConfigs_ObjectTypes FOREIGN KEY
	(
	ObjectType
	) REFERENCES dbo.ObjectTypes
	(
	ObjectType
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO


