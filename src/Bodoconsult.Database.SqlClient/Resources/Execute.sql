DECLARE @sql nvarchar(max) 

SET @sql= @Data+';'


IF (@sql IS NULL)
    THROW 50000, N'SQL not valid', 0;

PRINT @sql;  --For testing in SSMS
EXEC sp_executesql @sql;