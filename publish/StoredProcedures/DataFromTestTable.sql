create procedure DataFromTestTable
as
begin
    SELECT
        Id,
        [Name]
    FROM
        dbo.testTable
    ORDER BY 1 DESC
end