
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.SupporterGroups', 'U') IS NOT NULL  
DROP TABLE dbo.SupporterGroups
GO

CREATE TABLE [dbo].[SupporterGroups](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExternalId] [int] NULL,
	[CreationDate] [datetime2](7) NOT NULL DEFAULT (getdate()),
	[ModifiedDate] [datetime2](7) NOT NULL DEFAULT (getdate()),
	
    [GroupId] [int] NOT NULL,
	[SupporterId] [int] NOT NULL
	
	PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

IF OBJECT_ID('dbo.UQ_SupporterGroups_ExternalId', 'UQ') IS NOT NULL  
DROP INDEX dbo.UQ_SupporterGroups_ExternalId
GO

CREATE UNIQUE  NONCLUSTERED  INDEX UQ_SupporterGroups_ExternalId
    ON dbo.SupporterGroups ( ExternalId ASC )
    WHERE ExternalId is not null
GO

IF OBJECT_ID('dbo.TRG_UpdateSupporterGroupsModifiedDate', 'TR') IS NOT NULL  
DROP TRIGGER dbo.TRG_UpdateSupporterGroupsModifiedDate 
GO

CREATE TRIGGER dbo.trg_UpdateSupporterGroupsModifiedDate 
   ON  dbo.SupporterGroups 
   AFTER UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE dbo.SupporterGroups
    SET ModifiedDate = GETDATE()
    WHERE ID IN (SELECT DISTINCT ID FROM Inserted)
END
GO
