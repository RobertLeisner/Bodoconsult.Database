DECLARE @sql nvarchar(max) 

SET @sql= 'RESTORE FILELISTONLY FROM DISK = '''+@filename+''''


IF (@sql IS NULL)
    THROW 50000, N'Backup file does not exist', 0;

PRINT @sql;  --For testing in SSMS
EXEC sp_executesql @sql;