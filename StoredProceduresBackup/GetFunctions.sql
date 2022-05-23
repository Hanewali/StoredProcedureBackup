SELECT
    name AS [name],
	 type AS [type],
     SCHEMA_NAME(schema_id) AS [schema]
FROM
    sys.objects
WHERE
    type in ('IF', 'FN', 'AF', 'TF');