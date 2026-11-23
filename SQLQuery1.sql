USE HelpDeskDB;

SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

select * from Categories;
select * from dbo.AspNetUsers

SELECT DATABASEPROPERTYEX('HelpDeskDB', 'Collation') AS DatabaseCollation;

USE HelpDeskDB;

SELECT *
FROM Categories
ORDER BY Id;

SELECT Id, TicketNumber, Title, CategoryId
FROM Tickets
WHERE CategoryId IN (3, 4);


DELETE FROM Categories
WHERE Id IN (3, 4);


SELECT
    i.name AS IndexName,
    i.is_unique AS IsUnique,
    c.name AS ColumnName
FROM sys.indexes i
INNER JOIN sys.index_columns ic
    ON i.object_id = ic.object_id
    AND i.index_id = ic.index_id
INNER JOIN sys.columns c
    ON ic.object_id = c.object_id
    AND ic.column_id = c.column_id
WHERE i.object_id = OBJECT_ID('dbo.Tickets')
  AND c.name = 'TicketNumber';


SELECT
    COUNT(*) AS TicketCount,
    MAX(TRY_CONVERT(INT, SUBSTRING(TicketNumber, 4, LEN(TicketNumber))))
        AS HighestTicketNumber
FROM dbo.Tickets
WHERE TicketNumber LIKE 'HD-%';