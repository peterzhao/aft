
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
