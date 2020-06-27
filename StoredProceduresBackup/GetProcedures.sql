-- noinspection SqlNoDataSourceInspectionForFile

SELECT
    [schema] = OBJECT_SCHEMA_NAME([object_id]),
    [name]
FROM
    sys.procedures;