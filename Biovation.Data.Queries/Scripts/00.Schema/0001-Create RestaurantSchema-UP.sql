
IF NOT EXISTS ( SELECT  [name]
                FROM    sys.schemas
                WHERE   [name] = N'Rst' )
    EXEC('CREATE SCHEMA [Rst]');

--CREATE SCHEMA [Rst]
