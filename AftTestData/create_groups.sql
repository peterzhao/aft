
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.Groups', 'U') IS NOT NULL  
DROP TABLE dbo.Groups
GO

CREATE TABLE [dbo].[Groups](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExternalId] [int] NULL,
	[CreationDate] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[ModifiedDate] [datetime2](7) NOT NULL DEFAULT (getdate()),
	
    [Name] [nvarchar](100) NULL,
	[ReferenceName] [nvarchar](100) NULL,
	[Description] [nvarchar](MAX) NULL,
	[Notes] [nvarchar](MAX) NULL
	
	PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

IF OBJECT_ID('dbo.UQ_Groups_ExternalId', 'UQ') IS NOT NULL  
DROP INDEX dbo.UQ_Groups_ExternalId
GO

CREATE UNIQUE  NONCLUSTERED  INDEX UQ_Groups_ExternalId
    ON dbo.Groups ( ExternalId ASC )
    WHERE ExternalId is not null
GO

IF OBJECT_ID('dbo.TRG_UpdateGroupsModifiedDate', 'TR') IS NOT NULL  
DROP TRIGGER dbo.TRG_UpdateGroupsModifiedDate 
GO

CREATE TRIGGER dbo.trg_UpdateGroupsModifiedDate 
   ON  dbo.Groups 
   AFTER UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE dbo.Groups
    SET ModifiedDate = GETDATE()
    WHERE ID IN (SELECT DISTINCT ID FROM Inserted)
END
GO
