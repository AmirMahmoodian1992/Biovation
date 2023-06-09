
CREATE PROCEDURE [dbo].[ModifyAccessGroupUserGroup]
@Xml nvarchar(max),
@AccessGroupId int
AS
Declare  
			 @DocId INT,
			 @Message NVARCHAR(200)=N'عملیات با موفقیت انجام شد',
			 @Validate INT=1  ,
			 @Code AS NVARCHAR (15) = N'201';
			
		BEGIN
			SET LOCK_TIMEOUT 1800
			BEGIN TRY
			BEGIN TRANSACTION T1
			 DECLARE @Id AS INT
       			EXEC sp_xml_preparedocument @DocId OUTPUT, @Xml

        BEGIN

		DELETE FROM [dbo].AccessGroupUser WHERE AccessGroupId = @AccessGroupId

          INSERT INTO [dbo].[AccessGroupUser] (UserGroupId , AccessGroupId)

            SELECT Id , @AccessGroupId 
            FROM   OPENXML (@DocId, '/Root/UserGroup', 2) WITH (Id INT ) WHERE [Id] IS NOT NULL
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
					  set @Code = N'400';
				  END 
			END CATCH
END
select @Message [Message],@Validate Validate ,@Code Code
GO
