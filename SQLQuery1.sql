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