IF COL_LENGTH('Rst.ServeLog', 'Count') IS NULL
BEGIN
ALTER TABLE [Rst].[ServeLog]
ADD
[Count] [int] NULL
END
