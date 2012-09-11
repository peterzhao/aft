SELECT * FROM dbo.ObjectTypes where ObjectType = 'supporter_chapter'
IF @@ROWCOUNT = 0
BEGIN
  INSERT dbo.ObjectTypes (ObjectType) values ('supporter_chapter')
END
