SELECT
     name AS 'Function Name',
     SCHEMA_NAME(schema_id) AS 'Schema',
     type_desc AS 'Function Type', 
     create_date AS 'Created Date'
FROM
     sys.objects
WHERE
     type in ('FN', 'IF', 'FN', 'AF', 'FS', 'FT');