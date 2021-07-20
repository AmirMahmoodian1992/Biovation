CREATE PROCEDURE [dbo].[SelectServiceInstanceById]
@Id NVARCHAR(MAX)=NULL
AS
BEGIN

    IF(ISNULL(@Id,0) = 0)
    BEGIN
        SELECT [SI].[Id] ,
               [SI].[Name],
               [SI].[Version],
               [SI].[Ip] ,
               [SI].[Port],
		       [SI].[Description]
        FROM   [dbo].[ServiceInstance] as SI
    END
    ELSE
    BEGIN
   
        SELECT [SI].[Id],
               [SI].[Name],
               [SI].[Version],
               [SI].[Ip],
               [SI].[Port],
		       [SI].[Description]
         FROM   [dbo].[ServiceInstance] as SI
         WHERE  [SI].[Id] = @Id 
    END
END