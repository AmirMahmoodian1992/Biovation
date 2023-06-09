
CREATE PROCEDURE [dbo].[ModifyAccessGroupDeviceGroup]
@Xml nvarchar(max),
@AccessGroupId int
AS

--declare @Xml nvarchar(max)
--declare @AccessGroupId int
--set @Xml = N'<Root><DeviceGroup><Id>1002</Id><Name>گروه دو</Name><Devices /></DeviceGroup><DeviceGroup><Id>1003</Id><Name>گروه دو</Name><Devices /></DeviceGroup></Root>'
--set @AccessGroupId = 4

		Declare  
			 @DocId INT,
			 @Message NVARCHAR(200)=N'عملیات با موفقیت انجام شد',
			 @Validate INT=1  
			,@Code AS NVARCHAR (15) = N'201';
			
	BEGIN
			SET LOCK_TIMEOUT 1800
			BEGIN TRY
			BEGIN TRANSACTION T1
			 DECLARE @Id AS INT
       			EXEC sp_xml_preparedocument @DocId OUTPUT, @Xml

        BEGIN

		   DELETE FROM [dbo].AccessGroupDevice WHERE AccessGroupId = @AccessGroupId

          INSERT INTO [dbo].[AccessGroupDevice] (DeviceGroupId , AccessGroupId)

            SELECT Id , @AccessGroupId 
            FROM   OPENXML (@DocId, '/Root/DeviceGroup', 2) WITH (Id INT ) WHERE [Id] IS NOT NULL
        END
	
			EXEC sp_xml_removedocument @DocId	

			COMMIT TRANSACTION T1

			END TRY
			BEGIN CATCH 
			  IF (@@TRANCOUNT > 0)
			   BEGIN
				  ROLLBACK TRANSACTION T1
					  set @Validate = 0
					  set @Message = N'خطا در بانک اطلاعاتی' 
					  set @Code  = N'400';
				  END 
			END CATCH
END

select @Message [Message],@Validate Validate ,@Code Code
GO
