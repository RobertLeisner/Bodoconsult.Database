DECLARE @sql nvarchar(max) 


SELECT @sql= 'BACKUP DATABASE '
    + QUOTENAME(name)
    + ' TO DISK=@soutputfile WITH FORMAT, INIT, NAME = @sbackup_set_name, SKIP'
FROM sys.databases
WHERE name = @database;

IF (@sql IS NULL)
    THROW 50000, N'Database does not exist', 0;

PRINT @sql;  --For testing in SSMS
EXEC sp_executesql
    @sql,
    N'@sbackup_set_name nvarchar(128), @soutputfile nvarchar(900)',
    @sbackup_set_name = @backup_set_name,
    @soutputfile = @outputfile;