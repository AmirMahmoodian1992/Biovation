
CREATE PROCEDURE [dbo].[InsertDeviceGroupMember] 

@DeviceGroupMember nvarchar(max),
@DeviceGroupId int
AS
BEGIN
	Declare
	 @Message nvarchar(200)= N'ایجاد با موفقیت انجام گرفت'
	,@Validate int = 1  
	,@Code int= 201;
BEGIN TRY

BEGIN TRANSACTION T1 
	
	DELETE FROM [dbo].[DeviceGroupMember] 
		WHERE GroupId = @DeviceGroupId
	
DECLARE @Handle AS INT; -- The handle of the XML data, passed to sp_xml_preparedocument
DECLARE @Xml AS NVARCHAR(1000);

--SET @Xml = N'<Root>
--<DeviceGroupMember>
--<DeviceId>21</DeviceId>
--<GroupId>4005</GroupId>
--</DeviceGroupMember>
--<DeviceGroupMember>
--<DeviceId>2</DeviceId>
--<GroupId>4005</GroupId>
--</DeviceGroupMember></Root>'
Declare 
		@DeviceId int,
		@GroupId int
 
EXEC sys.sp_xml_preparedocument @Handle OUTPUT , @DeviceGroupMember; --Prepare a parsed document 

begin
	insert into [dbo].[DeviceGroupMember]
	   ([DeviceId]
           ,[GroupId]
         )
		   select   
DeviceId,
GroupId
	
FROM OPENXML(@Handle, '/Root/DeviceGroupMember', 2)
		with (
		DeviceId  int
		, GroupId       int
		
		)
	
	end

	EXEC sys.sp_xml_removedocument @Handle; --Remove the handle

	set @Code = SCOPE_IDENTITY()
COMMIT TRANSACTION T1
END TRY
BEGIN CATCH 
  IF (@@TRANCOUNT > 0)
   BEGIN
      ROLLBACK TRANSACTION T1
	  SET @Validate = 0
      SET @Message= N'ایجاد انجام نشد'
	  SET @Code = 400;
   END 
END CATCH
select @Message [Message],@Code Id,@Validate Validate,@Code Code
END

GO
