Create PROCEDURE [dbo].[SelectServiceInstanceById]
@Id NVARCHAR(MAX)=null
AS
BEGIN

 SELECT [SI].[Id] As Data_Id,
           [SI].[Name] As Data_Name,
           [SI].[Version] AS Data_Version,
		   [SI].[Description] AS Data_Description,
		   [SI].[Health] AS Data_Health
    FROM   [dbo].[ServiceInstance] as SI
    WHERE  [SI].[Id] = @Id
END