Create PROCEDURE [dbo].[SelectServiceInstanceById]
@Id NVARCHAR(MAX)=null
AS
BEGIN

 SELECT [SI].[Id] As Data_Id,
           [SI].[Name] As Data_Name,
           [SI].[Version] AS Data_Version,
           [SI].[Ip] AS Data_Ip,
           [SI].[Port] AS Data_Port,
		   [SI].[Description] AS Data_Description
    FROM   [dbo].[ServiceInstance] as SI
    WHERE  [SI].[Id] = @Id
END