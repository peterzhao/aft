--DROP TABLE [dbo].[Supporters]
CREATE TABLE [dbo].[Supporters]
(
  [Id] INT NOT NULL PRIMARY KEY IDENTITY
, [AFT_Created] DATETIME2 NOT NULL DEFAULT GETDATE()
, [AFT_LastModified] DATETIME2 NOT NULL DEFAULT GETDATE()
, [supporter_KEY] INT NULL
, [Last_Modified] DATETIME2 NULL
, [Date_Created] DATETIME2 NULL
, [Title] NVARCHAR(50) NULL
, [First_Name] NVARCHAR(50) NULL
, [MI] NVARCHAR(50) NULL
, [Last_Name] NVARCHAR(50) NULL
, [Suffix] NVARCHAR(50) NULL
, [Email] NVARCHAR(50) NULL
, [Password] NVARCHAR(50) NULL
, [Receive_Email] TINYINT NULL
, [Email_Preference] NVARCHAR(50) NULL
, [Last_Bounce] DATETIME2 NULL
, [Receive_Phone_Blasts] TINYINT NULL
, [Phone] NVARCHAR(50) NULL
, [Cell_Phone] NVARCHAR(50) NULL
, [Phone_Provider] NVARCHAR(50) NULL
, [Work_Phone] NVARCHAR(50) NULL
, [Pager] NVARCHAR(50) NULL
, [Home_Fax] NVARCHAR(50) NULL
, [Work_Fax] NVARCHAR(50) NULL
, [Street] NVARCHAR(50) NULL
, [Street_2] NVARCHAR(50) NULL
, [Street_3] NVARCHAR(50) NULL
, [City] NVARCHAR(50) NULL
, [State] NVARCHAR(50) NULL
, [Zip] NVARCHAR(50) NULL
, [PRIVATE_Zip_Plus_4] NVARCHAR(50) NULL
, [County] NVARCHAR(50) NULL
, [District] NVARCHAR(50) NULL
, [Country] NVARCHAR(50) NULL
, [Latitude] FLOAT NULL
, [Longitude] FLOAT NULL
, [Organization] NVARCHAR(50) NULL
, [Department] NVARCHAR(50) NULL
, [Occupation] NVARCHAR(50) NULL
, [Instant_Messenger_Service] NVARCHAR(50) NULL
, [Instant_Messenger_Name] NVARCHAR(50) NULL
, [Web_Page] NVARCHAR(50) NULL
, [Alternative_Email] NVARCHAR(50) NULL
, [Other_Data_1] NVARCHAR(50) NULL
, [Other_Data_2] NVARCHAR(50) NULL
, [Other_Data_3] NVARCHAR(50) NULL
, [Notes] NVARCHAR(255) NULL
, [Source] NVARCHAR(50) NULL
, [Source_Details] NVARCHAR(50) NULL
, [Source_Tracking_Code] NVARCHAR(50) NULL
, [Tracking_Code] NVARCHAR(50) NULL
, [Status] NVARCHAR(50) NULL
, [uid] NVARCHAR(50) NULL
, [Timezone] NVARCHAR(50) NULL
, [Language_Code] NVARCHAR(50) NULL
, [CustomString0] NVARCHAR(50) NULL
, [CustomString1] NVARCHAR(50) NULL
, [CustomString2] NVARCHAR(50) NULL
, [CustomString3] NVARCHAR(50) NULL
, [CustomString4] NVARCHAR(50) NULL
, [CustomString5] NVARCHAR(50) NULL
, [CustomString6] NVARCHAR(50) NULL
, [CustomString7] NVARCHAR(50) NULL
, [CustomString8] NVARCHAR(50) NULL
, [CustomString9] NVARCHAR(50) NULL
, [CustomInteger0] INT NULL
, [CustomInteger1] INT NULL
, [CustomInteger2] INT NULL
, [CustomInteger3] INT NULL
, [CustomInteger4] INT NULL
, [CustomBoolean0] BIT NULL
, [CustomBoolean1] BIT NULL
, [CustomBoolean2] BIT NULL
, [CustomBoolean3] BIT NULL
, [CustomBoolean4] BIT NULL
, [CustomBoolean5] BIT NULL
, [CustomBoolean6] BIT NULL
, [CustomBoolean7] BIT NULL
, [CustomBoolean8] BIT NULL
, [CustomBoolean9] BIT NULL
)
GO