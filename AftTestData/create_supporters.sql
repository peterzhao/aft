USE [AFT]
GO

/****** Object:  Table [dbo].[Supporters]    Script Date: 08/01/2012 10:45:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.Supporters', 'U') IS NOT NULL  
DROP TABLE dbo.Supporters
GO

CREATE TABLE [dbo].[Supporters](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExternalId] [int] NULL,
	[CreationDate] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[ModifiedDate] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[Title] [nvarchar](50) NULL,
	[First_Name] [nvarchar](50) NULL,
	[MI] [nvarchar](50) NULL,
	[Last_Name] [nvarchar](50) NULL,
	[Suffix] [nvarchar](50) NULL,
	[Email] [nvarchar](50) NULL,
	[Password] [nvarchar](50) NULL,
	[Receive_Email] [tinyint] NULL,
	[Email_Preference] [nvarchar](50) NULL,
	[Last_Bounce] [datetime2](7) NULL,
	[Receive_Phone_Blasts] [tinyint] NULL,
	[Phone] [nvarchar](50) NULL,
	[Cell_Phone] [nvarchar](50) NULL,
	[Phone_Provider] [nvarchar](50) NULL,
	[Work_Phone] [nvarchar](50) NULL,
	[Pager] [nvarchar](50) NULL,
	[Home_Fax] [nvarchar](50) NULL,
	[Work_Fax] [nvarchar](50) NULL,
	[Street] [nvarchar](50) NULL,
	[Street_2] [nvarchar](50) NULL,
	[Street_3] [nvarchar](50) NULL,
	[City] [nvarchar](50) NULL,
	[State] [nvarchar](50) NULL,
	[Zip] [nvarchar](50) NULL,
	[PRIVATE_Zip_Plus_4] [nvarchar](50) NULL,
	[County] [nvarchar](50) NULL,
	[District] [nvarchar](50) NULL,
	[Country] [nvarchar](50) NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL,
	[Organization] [nvarchar](50) NULL,
	[Department] [nvarchar](50) NULL,
	[Occupation] [nvarchar](50) NULL,
	[Instant_Messenger_Service] [nvarchar](50) NULL,
	[Instant_Messenger_Name] [nvarchar](50) NULL,
	[Web_Page] [nvarchar](50) NULL,
	[Alternative_Email] [nvarchar](50) NULL,
	[Other_Data_1] [nvarchar](50) NULL,
	[Other_Data_2] [nvarchar](50) NULL,
	[Other_Data_3] [nvarchar](50) NULL,
	[Notes] [nvarchar](255) NULL,
	[Source] [nvarchar](50) NULL,
	[Source_Details] [nvarchar](50) NULL,
	[Source_Tracking_Code] [nvarchar](50) NULL,
	[Tracking_Code] [nvarchar](50) NULL,
	[Status] [nvarchar](50) NULL,
	[Timezone] [nvarchar](50) NULL,
	[Language_Code] [nvarchar](50) NULL,
	[CustomString0] [nvarchar](50) NULL,
	[CustomString1] [nvarchar](50) NULL,
	[CustomString2] [nvarchar](50) NULL,
	[CustomString3] [nvarchar](50) NULL,
	[CustomString4] [nvarchar](50) NULL,
	[CustomString5] [nvarchar](50) NULL,
	[CustomString6] [nvarchar](50) NULL,
	[CustomString7] [nvarchar](50) NULL,
	[CustomString8] [nvarchar](50) NULL,
	[CustomString9] [nvarchar](50) NULL,
	[CustomInteger0] [int] NULL,
	[CustomInteger1] [int] NULL,
	[CustomInteger2] [int] NULL,
	[CustomInteger3] [int] NULL,
	[CustomInteger4] [int] NULL,
	[CustomBoolean0] [bit] NULL,
	[CustomBoolean1] [bit] NULL,
	[CustomBoolean2] [bit] NULL,
	[CustomBoolean3] [bit] NULL,
	[CustomBoolean4] [bit] NULL,
	[CustomBoolean5] [bit] NULL,
	[CustomBoolean6] [bit] NULL,
	[CustomBoolean7] [bit] NULL,
	[CustomBoolean8] [bit] NULL,
	[CustomBoolean9] [bit] NULL,
	[CustomDateTime0] [datetime2](7) NULL
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

IF OBJECT_ID('dbo.UQ_Supporters_supporter_KEY', 'UQ') IS NOT NULL  
DROP INDEX dbo.UQ_Supporters_ExternalId
GO

CREATE UNIQUE  NONCLUSTERED  INDEX UQ_Supporters_ExternalId
    ON dbo.Supporters ( ExternalId ASC )
    WHERE ExternalId is not null
GO


IF OBJECT_ID('dbo.trg_UpdateSupportersModifiedDate', 'TR') IS NOT NULL  
DROP TRIGGER dbo.trg_UpdateSupportersModifiedDate 
GO

CREATE TRIGGER dbo.trg_UpdateSupportersModifiedDate 
   ON  dbo.Supporters 
   AFTER UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE dbo.Supporters
    SET ModifiedDate = GETDATE()
    WHERE ID IN (SELECT DISTINCT ID FROM Inserted)
END
GO
