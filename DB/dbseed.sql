INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Email','Email' ,'string', 'aftWins');

INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','First_Name','First_Name' ,'string', 'onlyIfBlank');

INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Last_Name','Last_Name' ,'string', 'onlyIfBlank');

INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Last_Modified','SalsaLastModified' ,'dateTime', 'readOnly');




--INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
--VALUES('supporter','Chapter_KEY', 'Chapter_KEY', 'int', '');

	
--INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType])
--VALUES('supporter','CustomDateTime0', 'CustomDateTime0', 'datetime');

GO
INSERT INTO [dbo].[SyncConfigs]([ObjectType],[SyncDirection] ,[Order]) VALUES('supporter' ,'import',1)
INSERT INTO [dbo].[SyncConfigs]([ObjectType],[SyncDirection] ,[Order]) VALUES('supporter' ,'export',2)
GO



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF OBJECT_ID('dbo.SalsaToAftQueue_Supporter', 'U') IS NOT NULL
DROP TABLE dbo.SalsaToAftQueue_Supporter
GO

CREATE TABLE [dbo].SalsaToAftQueue_Supporter(
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [SalsaKey] [int] NOT NULL,
        [First_Name] [nvarchar](50) NULL,
        [Last_Name] [nvarchar](50) NULL,
        [Email] [nvarchar](50) NULL,
        [Chapter_KEY] [int] NULL,
        [CustomDateTime0] [datetime] NULL,
        [CustomBoolean0] [bit] NULL,
        [CustomInteger0] [bit] NULL,
        [SalsaLastModified] [datetime] null
 
PRIMARY KEY CLUSTERED
(
        [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF OBJECT_ID('dbo.AftToSalsaQueue_Supporter', 'U') IS NOT NULL
DROP TABLE dbo.AftToSalsaQueue_Supporter
GO

CREATE TABLE [dbo].AftToSalsaQueue_Supporter(
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [SalsaKey] [int] NOT NULL,
        [First_Name] [nvarchar](50) NULL,
        [Last_Name] [nvarchar](50) NULL,
        [Email] [nvarchar](50) NULL,
		[Chapter_KEY] [int] NULL,
        [CustomDateTime0] [datetime] NULL,
        [CustomBoolean0] [bit] NULL,
        [CustomInteger0] [bit] NULL

PRIMARY KEY CLUSTERED
(
        [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

GO
