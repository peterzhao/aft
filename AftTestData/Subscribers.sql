CREATE TABLE [dbo].[Subscribers]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
	[Created] DATETIME2 NOT NULL DEFAULT GETDATE(),
	[LastModified] DATETIME2 NOT NULL DEFAULT GETDATE(),
	[EmailAddress] NVARCHAR(50) NOT NULL,
	[Title] NVARCHAR(16) NULL,
	[FirstName] NVARCHAR(50) NULL, 
	[MiddleInitial] NVARCHAR(20) NULL, 
	[LastName] NVARCHAR(50) NULL, 
	[Suffix] NVARCHAR(16) NULL,
	[Phone] NVARCHAR(16) NULL,
	[Street] NVARCHAR(64) NULL,
	[Street_2] NVARCHAR(64) NULL,
	[Street_3] NVARCHAR(128) NULL,
	[City] NVARCHAR(32) NULL,
	[State] NVARCHAR(32) NULL,
	[Country] NVARCHAR(50) NULL,
	[Zip] NVARCHAR(10) NULL,
	[Organization] [nvarchar](50) NULL,
	[salsa_SupporterKey] [int] NULL,
	[salsa_ChapterKey] [int] NULL,
	[salsa_Password] NVARCHAR(50) NULL
)
