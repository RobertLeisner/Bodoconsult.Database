DECLARE @sql nvarchar(max) 

/*
set @Sql = '' 

select  @Sql = @Sql + 'kill ' + convert(char(10), spid) + '; ' 
from    master.dbo.sysprocesses 
where   db_name(dbid) = @database
     and 
     dbid <> 0 
     and 
     spid <> @@spid 
exec(@Sql)
*/

set @Sql = ''

SELECT @sql= 'IF ISNULL(DB_ID('''+[name]+'''),0) > 0 BEGIN ALTER DATABASE '
    + QUOTENAME([name])
    + ' SET SINGLE_USER WITH ROLLBACK IMMEDIATE; exec sp_detach_db '+ QUOTENAME([name])+', true; END;'
FROM sys.databases
WHERE [name] = @database;

--SELECT @sql= 'IF ISNULL(DB_ID('''+[name]+'''),0) > 0 BEGIN ALTER DATABASE '
--    + QUOTENAME([name])
--    + ' SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE '+ QUOTENAME([name])+'; END;'
--FROM sys.databases
--WHERE [name] = @database;

IF (@sql IS NULL)
    THROW 50000, N'Database does not exist', 0;

PRINT @sql;  --For testing in SSMS
EXEC sp_executesql @sql;