
delete from FieldMappings
delete from SyncConfigs

-- supporter tables

INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Email','Email' ,'string', 'primaryKey');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Last_Modified','Last_Modified' ,'dateTime', 'readOnly');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Date_Created','Date_Created' ,'dateTime', 'readOnly');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','chapter_KEY','Chapter_KEY' ,'int', 'writeOnlyNewMembership');

INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Title','Title' ,'string', 'onlyIfBlank');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','First_Name','First_Name' ,'string', 'onlyIfBlank');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Last_Name','Last_Name' ,'string', 'onlyIfBlank');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','MI','MI' ,'string', 'onlyIfBlank');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Suffix','Suffix' ,'string', 'onlyIfBlank');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Phone','Phone' ,'string', 'onlyIfBlank');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Street','Street' ,'string', 'onlyIfBlank');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Street_2','Street_2' ,'string', 'onlyIfBlank');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Street_3','Street_3' ,'string', 'onlyIfBlank');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','City','City' ,'string', 'onlyIfBlank');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','State','State' ,'string', 'onlyIfBlank');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Zip','Zip' ,'string', 'onlyIfBlank');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','PRIVATE_Zip_Plus_4','PRIVATE_Zip_Plus_4' ,'string', 'onlyIfBlank');

INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Organization','Organization' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Department','Department' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Occupation','Occupation' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','cdb_guid','IndividualID' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','leadership_code','Leadership_Code' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','membership_status_2','MCODE' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','contact_preferences','MAIL' ,'string', 'aftWins');

INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','aft_local_number','LOCALNBR' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','aft_local_name','Local_Name' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','pub_at','AT_Pub' ,'bool', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','pub_psrp','PSRP_Pub' ,'bool', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','pub_pe','PE_Pub' ,'bool', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','pub_hed','HED_Pub' ,'bool', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','pub_health','Health_Pub' ,'bool', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','cdb_match_method','Method_Match' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','cdb_match_date','AFT_Match_DateTime' ,'datetime', 'aftWins');

INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','local_job_classification','Job_Class' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','most_recent_assesment','Most_Recent_Assement_Rating' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','local_job_classification','Local_Job_Classification' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','salary','Salary' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','work_unit','Work_Unit' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','worksite','Worksite' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','worksite_area','Worksite_Area' ,'string', 'aftWins');

INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','payment_enrolled','Payment_Enrolled' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','enrollment_type','Enrollment_Type' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','local_dues_category','Local_Dues_Category' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','employer','Employer' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','non_member_type','Non_Member_Type' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','union_status','Union_Status' ,'string', 'aftWins');

INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','building_representative','Building_Rep' ,'bool', 'aftWins');

INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','officer_info','Officer_Info' ,'string', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','key_contact','Key_Contact' ,'bool', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','pac_contributor','PAC_Contributor' ,'bool', 'aftWins');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','pac_date','PAC_Date' ,'string', 'aftWins');

GO

INSERT INTO [dbo].[SyncConfigs]([ObjectType],[SyncDirection] ,[Order]) VALUES('supporter' ,'export',1)
INSERT INTO [dbo].[SyncConfigs]([ObjectType],[SyncDirection] ,[Order]) VALUES('supporter' ,'import',2)
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
		[Title] varchar(200) null,
		[First_Name] varchar(200) null,
		[MI] varchar(200) null,
		[Last_Name] varchar(200) null,
		[Suffix] varchar(200) null,
		[Email] varchar(200)  null,
		[Date_Created] datetime null,
		[Last_Modified] datetime null,
		[Phone] varchar(200) null,
		[Street] varchar(200) null,
		[Street_2] varchar(200) null,
		[Street_3] varchar(200) null,
		[City] varchar(200) null,
		[State] varchar(200) null,
        [Zip] varchar(200) null,
		[PRIVATE_Zip_Plus_4] varchar(200) null,
		[Organization] varchar(200) null,
		[Department] varchar(200) null,
		[Occupation] varchar(200) null,
		[IndividualID] varchar(200) null,
		[Leadership_Code] varchar(200) null,
		[MCODE] varchar(200) null,
		[MAIL] varchar(200) null,
		[LOCALNBR] varchar(200) null,
		[Local_Name] varchar(200) null,
		[AT_Pub] bit null,
		[PSRP_Pub] bit null,
		[PE_Pub] bit null,
		[HED_Pub] bit null,
		[Health_Pub] bit null,
		[Method_Match] varchar(200) null,
		[AFT_Match_DateTime] datetime null,
		[Job_Class] varchar(200) null,
		[Most_Recent_Assement_Rating] varchar(200) null,
		[Local_Job_Classification] varchar(200) null,
		[Salary] varchar(200) null,
		[Work_Unit] varchar(200) null,
		[Worksite] varchar(200) null,
		[Worksite_Area] varchar(200) null,
		[Payment_Enrolled] varchar(200) null,
		[Enrollment_Type] varchar(200) null,
		[Local_Dues_Category] varchar(200) null,
		[Employer] varchar(200) null,
		[Non_Member_Type] varchar(200) null,
		[Union_Status] varchar(200) null,
		[Building_Rep] bit null,
		[Officer_Info] varchar(200) null,
		[Key_Contact] bit null,
		[PAC_Contributor] bit null,
		[PAC_Date] nvarchar(200) null,
		[Cdate] datetime null default(getdate()),
		[ProcessedDate] datetime null,
		[Status] nvarchar(200) null
 
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
        [SalsaKey] [int] NULL,
        [Chapter_KEY] int NULL,
		[Title] varchar(200) null,
		[First_Name] varchar(200) null,
		[MI] varchar(200) null,
		[Last_Name] varchar(200) null,
		[Suffix] varchar(200) null,
		[Email] varchar(200) null,		
		[Phone] varchar(200) null,
		[Street] varchar(200) null,
		[Street_2] varchar(200) null,
		[Street_3] varchar(200) null,
		[City] varchar(200) null,
		[State] varchar(200) null,
        [Zip] varchar(200) null,
		[PRIVATE_Zip_Plus_4] varchar(200) null,
		[Organization] varchar(200) null,
		[Department] varchar(200) null,
		[Occupation] varchar(200) null,
		[IndividualID] varchar(200) null,
		[Leadership_Code] varchar(200) null,
		[MCODE] varchar(200) null,
		[MAIL] varchar(200) null,
		[LOCALNBR] varchar(200) null,
		[Local_Name] varchar(200) null,
		[AT_Pub] bit null,
		[PSRP_Pub] bit null,
		[PE_Pub] bit null,
		[HED_Pub] bit null,
		[Health_Pub] bit null,
		[Method_Match] varchar(200) null,
		[AFT_Match_DateTime] datetime null,
		[Job_Class] varchar(200) null,
		[Most_Recent_Assement_Rating] varchar(200) null,
		[Local_Job_Classification] varchar(200) null,
		[Salary] varchar(200) null,
		[Work_Unit] varchar(200) null,
		[Worksite] varchar(200) null,
		[Worksite_Area] varchar(200) null,
		[Payment_Enrolled] varchar(200) null,
		[Enrollment_Type] varchar(200) null,
		[Local_Dues_Category] varchar(200) null,
		[Employer] varchar(200) null,
		[Non_Member_Type] varchar(200) null,
		[Union_Status] varchar(200) null,
		[Building_Rep] bit null,
		[Officer_Info] varchar(200) null,
		[Key_Contact] bit null,
		[PAC_Contributor] bit null,
		[PAC_Date] nvarchar(200) null,
		[Cdate] datetime null default(getdate()),
		[ProcessedDate] datetime null,
		[Status] nvarchar(200) null

PRIMARY KEY CLUSTERED
(
        [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- supporter history tables

IF OBJECT_ID('dbo.SalsaToAftQueue_Supporter_History', 'U') IS NOT NULL
DROP TABLE dbo.SalsaToAftQueue_Supporter_History
GO

CREATE TABLE [dbo].SalsaToAftQueue_Supporter_History(
        [HistoryId] [int] IDENTITY(1,1) NOT NULL,
        [Id] [int] NOT NULL,
        [SalsaKey] [int] NOT NULL,
		[Title] varchar(200) null,
		[First_Name] varchar(200) null,
		[MI] varchar(200) null,
		[Last_Name] varchar(200) null,
		[Suffix] varchar(200) null,
		[Email] varchar(200)  null,
		[Date_Created] datetime null,
		[Last_Modified] datetime null,
		[Phone] varchar(200) null,
		[Street] varchar(200) null,
		[Street_2] varchar(200) null,
		[Street_3] varchar(200) null,
		[City] varchar(200) null,
		[State] varchar(200) null,
        [Zip] varchar(200) null,
		[PRIVATE_Zip_Plus_4] varchar(200) null,
		[Organization] varchar(200) null,
		[Department] varchar(200) null,
		[Occupation] varchar(200) null,
		[IndividualID] varchar(200) null,
		[Leadership_Code] varchar(200) null,
		[MCODE] varchar(200) null,
		[MAIL] varchar(200) null,
		[LOCALNBR] varchar(200) null,
		[Local_Name] varchar(200) null,
		[AT_Pub] bit null,
		[PSRP_Pub] bit null,
		[PE_Pub] bit null,
		[HED_Pub] bit null,
		[Health_Pub] bit null,
		[Method_Match] varchar(200) null,
		[AFT_Match_DateTime] datetime null,
		[Job_Class] varchar(200) null,
		[Most_Recent_Assement_Rating] varchar(200) null,
		[Local_Job_Classification] varchar(200) null,
		[Salary] varchar(200) null,
		[Work_Unit] varchar(200) null,
		[Worksite] varchar(200) null,
		[Worksite_Area] varchar(200) null,
		[Payment_Enrolled] varchar(200) null,
		[Enrollment_Type] varchar(200) null,
		[Local_Dues_Category] varchar(200) null,
		[Employer] varchar(200) null,
		[Non_Member_Type] varchar(200) null,
		[Union_Status] varchar(200) null,
		[Building_Rep] bit null,
		[Officer_Info] varchar(200) null,
		[Key_Contact] bit null,
		[PAC_Contributor] bit null,
		[PAC_Date] nvarchar(200) null,
		[Cdate] datetime null,
		[ProcessedDate] datetime null,
		[Status] nvarchar(200) null
 
PRIMARY KEY CLUSTERED
(
        [HistoryId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.AftToSalsaQueue_Supporter_History', 'U') IS NOT NULL
DROP TABLE dbo.AftToSalsaQueue_Supporter_History
GO

CREATE TABLE [dbo].AftToSalsaQueue_Supporter_History(

         [HistoryId] [int] IDENTITY(1,1) NOT NULL,
         [Id] [int] NOT NULL,

        [SalsaKey] [int] NULL,
        [Chapter_KEY] int NULL,
		[Title] varchar(200) null,
		[First_Name] varchar(200) null,
		[MI] varchar(200) null,
		[Last_Name] varchar(200) null,
		[Suffix] varchar(200) null,
		[Email] varchar(200) null,		
		[Phone] varchar(200) null,
		[Street] varchar(200) null,
		[Street_2] varchar(200) null,
		[Street_3] varchar(200) null,
		[City] varchar(200) null,
		[State] varchar(200) null,
        [Zip] varchar(200) null,
		[PRIVATE_Zip_Plus_4] varchar(200) null,
		[Organization] varchar(200) null,
		[Department] varchar(200) null,
		[Occupation] varchar(200) null,
		[IndividualID] varchar(200) null,
		[Leadership_Code] varchar(200) null,
		[MCODE] varchar(200) null,
		[MAIL] varchar(200) null,
		[LOCALNBR] varchar(200) null,
		[Local_Name] varchar(200) null,
		[AT_Pub] bit null,
		[PSRP_Pub] bit null,
		[PE_Pub] bit null,
		[HED_Pub] bit null,
		[Health_Pub] bit null,
		[Method_Match] varchar(200) null,
		[AFT_Match_DateTime] datetime null,
		[Job_Class] varchar(200) null,
		[Most_Recent_Assement_Rating] varchar(200) null,
		[Local_Job_Classification] varchar(200) null,
		[Salary] varchar(200) null,
		[Work_Unit] varchar(200) null,
		[Worksite] varchar(200) null,
		[Worksite_Area] varchar(200) null,
		[Payment_Enrolled] varchar(200) null,
		[Enrollment_Type] varchar(200) null,
		[Local_Dues_Category] varchar(200) null,
		[Employer] varchar(200) null,
		[Non_Member_Type] varchar(200) null,
		[Union_Status] varchar(200) null,
		[Building_Rep] bit null,
		[Officer_Info] varchar(200) null,
		[Key_Contact] bit null,
		[PAC_Contributor] bit null,
		[PAC_Date] nvarchar(200) null,
		[Cdate] datetime null,
		[ProcessedDate] datetime null,
		[Status] nvarchar(200) null

PRIMARY KEY CLUSTERED
(
        [HistoryId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- supporter_chapter

INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter_chapter','supporter_KEY','SupporterKey' ,'int', 'readOnly');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter_chapter','chapter_KEY','ChapterKey' ,'int', 'readOnly');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter_chapter','unsubscribed','Unsubscribed' ,'int', 'readOnly');

INSERT INTO [dbo].[SyncConfigs]([ObjectType],[SyncDirection] ,[Order]) VALUES('supporter_chapter', 'import', 3)
GO

IF OBJECT_ID('dbo.SalsaToAftQueue_Supporter_Chapter', 'U') IS NOT NULL
DROP TABLE dbo.SalsaToAftQueue_Supporter_Chapter
GO

CREATE TABLE [dbo].SalsaToAftQueue_Supporter_Chapter (
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [SalsaKey] [int] NOT NULL,
		[Cdate] datetime NULL default(getdate()),
		[ProcessedDate] datetime NULL,
		[Status] nvarchar(200) NULL,
		[SupporterKey] int NULL,
		[ChapterKey] int NULL,
		[Unsubscribed] int NULL
PRIMARY KEY CLUSTERED
(
        [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

GO


IF OBJECT_ID('dbo.SalsaToAftQueue_Supporter_Chapter_History', 'U') IS NOT NULL
DROP TABLE dbo.SalsaToAftQueue_Supporter_Chapter_History
GO

CREATE TABLE [dbo].SalsaToAftQueue_Supporter_Chapter_History (
        [HistoryId] [int] IDENTITY(1,1) NOT NULL,
        [Id] [int] NOT NULL,
        [SalsaKey] [int] NOT NULL,
		[Cdate] datetime NULL,
		[ProcessedDate] datetime NULL,
		[Status] nvarchar(200) NULL,
		[SupporterKey] int NULL,
		[ChapterKey] int NULL,
		[Unsubscribed] int NULL
 
PRIMARY KEY CLUSTERED
(
        [HistoryId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.SALSAChapters', 'U') IS NOT NULL
DROP TABLE [dbo].[SALSAChapters]
GO

CREATE TABLE [dbo].[SALSAChapters](
	[ChapterKey] [int] NOT NULL,
	[LocalUnionNumber] [varchar](5) NOT NULL,
 CONSTRAINT [PK_SALSAChapters] PRIMARY KEY CLUSTERED 
(
	[ChapterKey] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (44, '99998')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (339, '01567')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (340, '08006')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (341, '03550')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (342, '80370')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (344, '00943')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (347, '03732')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (349, '00915')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (352, '4200A')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (353, '01605')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (355, '02999')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (356, '06217')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (357, '01085')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (359, '01936')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (360, '07463')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (362, '00067')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (368, '08020')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (371, '08029')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (372, '02260')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (373, '06047')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (374, '08004')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (375, '06093')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (376, '06196')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (377, '02143')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (378, '01559')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (379, '00691')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (380, '08016')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (381, '04996')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (383, '06732')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (384, '02277')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (431, '01521')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (432, '01533')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (433, '03220')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (434, '04531')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (435, '08041')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (436, 'PEGTX')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (437, '04524')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (438, '08039')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (439, '04253')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (441, '04996')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (442, '03544')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (454, '02222')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (455, '00061')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (457, '00002')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (458, '08043')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (459, '00495')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (460, '00527')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (461, '04100')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (462, '01974')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (464, '04995')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (465, '04536')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (466, '04343')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (467, '00006')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (468, '00001')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (469, '02431')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (470, '02222')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (471, '05040')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (472, '03326')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (473, '04848')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (474, '02048')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (475, '03483')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (476, '03038')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (477, '02401')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (478, '08028')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (479, '01581')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (480, '03550')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (481, '02030')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (482, '07426')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (483, '07429')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (484, '03275')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (485, '02415')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (493, '08011')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (494, '00300')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (496, '02279')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (497, '08019')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (499, '08019')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (500, '08024')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (501, '00059')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (502, '04326')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (503, '06197')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (504, '05017')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (505, '08033')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (506, '04053')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (507, '00003')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (508, '07457')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (509, '08004')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (510, '06427')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (511, '00420')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (513, '06345')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (515, '06346')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (516, '06056')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (517, '07402')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (518, '01975')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (519, '03456')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (521, '2334R')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (522, '02115')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (523, '02026')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (524, '08005')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (525, '01931')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (526, '08018')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (527, '04518')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (528, '08018')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (529, '08019')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (530, '04008')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (531, '08028')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (532, '01605')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (533, '08045')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (534, '08027')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (535, '9539R')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (536, '04660')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (537, '08035')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (545, '03922')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (546, '08046')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (547, '08047')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (548, '08036')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (550, '04900')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (583, '08020')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (584, '00933')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (585, '00111')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (586, '00481')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (587, '04539')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (632, '06397')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (745, '06329')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (746, '04632')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (818, '01766')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (839, '99999')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (842, '05221')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (933, '01420')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (941, '02121')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (946, '01037')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (947, 'ORGTX')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (954, '01603')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (966, '08012')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (970, '05047')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (973, '06732')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (975, '00279')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (976, '04529')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (984, '03550')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (990, '07432')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1002, '08042')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1025, '06455')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1027, '00477')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1029, '09102')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1030, '06244')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1036, 'PEGTX')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1040, '00231')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1047, '08021')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1049, '03456')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1057, '01052')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1062, '08033')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1077, '06290')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1110, '02341')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1111, '08020')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1112, '01794')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1113, '01660')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1116, '04429')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1159, '00231')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1204, '02415')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1205, '08023')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1211, '02415')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1241, '02187')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1242, '00400')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1249, '04000')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1256, '01520')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1257, '02394')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1259, '02701')
INSERT INTO [dbo].[SALSAChapters] ([ChapterKey], [LocalUnionNumber]) VALUES (1260, '02006')

