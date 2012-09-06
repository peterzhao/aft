INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Email','Email' ,'string', 'aftWins');

INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','First_Name','First_Name' ,'string', 'onlyIfBlank');

INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
VALUES('supporter','Last_Name','Last_Name' ,'string', 'onlyIfBlank');

--INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType], [MappingRule])
--VALUES('supporter','Chapter_KEY', 'Chapter_KEY', 'int', '');

	
--INSERT INTO [dbo].[FieldMappings]([ObjectType],[SalsaField],[AftField],[DataType])
--VALUES('supporter','CustomDateTime0', 'CustomDateTime0', 'datetime');

GO
