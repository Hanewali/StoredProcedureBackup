SELECT
     name AS [name],
     SCHEMA_NAME(schema_id) AS [schema]
FROM
     sys.objects
WHERE
     type in ('FN', 'IF', 'FN', 'AF', 'FS', 'FT');