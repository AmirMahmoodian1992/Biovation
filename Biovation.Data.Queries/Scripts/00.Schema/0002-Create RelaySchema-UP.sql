IF NOT EXISTS ( SELECT  [name]
                FROM    sys.schemas
                WHERE   [name] = N'Rly' )
    EXEC('CREATE SCHEMA [Rly]');