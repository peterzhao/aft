-- The query below will return the clustered indexes dbo.ImporterLogs
SELECT *
FROM sys.indexes 
WHERE index_id = 1 
  AND object_id = OBJECT_ID('dbo.ImporterLogs', 'U')

-- If there are no clustered indexes, create one...
IF @@ROWCOUNT = 0
BEGIN
  CREATE CLUSTERED INDEX IX_ImporterLogs ON dbo.ImporterLogs (time_stamp)
END
