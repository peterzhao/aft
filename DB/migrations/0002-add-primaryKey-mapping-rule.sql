INSERT INTO [AftDemo].[dbo].[AftToSalsaQueue_Supporter]
           ([SalsaKey]
           ,[First_Name]
           ,[Last_Name]
           ,[Email]
           ,[Chapter_KEY]
           ,[CustomDateTime0]
           ,[CustomBoolean0]
           ,[CustomInteger0])
     VALUES
           (0
           ,'jim'
           ,'foo'
           ,'jfoo@abc.com'
           ,0
           ,'2008-01-15'
           ,1
           ,0)
GO


Insert into MappingRules(MappingRule) values('primaryKey')