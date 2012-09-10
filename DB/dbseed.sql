
delete from FieldMappings
delete from SyncConfigs

INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Email','Email' ,'string', 'primaryKey');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Last_Modified','Last_Modified' ,'dateTime', 'readOnly');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Date_Created','Date_Created' ,'dateTime', 'readOnly');
INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','chapter_KEY','Chapter_KEY' ,'int', 'aftWins');


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
        [Chapter_KEY] int NULL,
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
		[PAC_Date] nvarchar(200) null
 
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
		[PAC_Date] nvarchar(200) null

PRIMARY KEY CLUSTERED
(
        [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

