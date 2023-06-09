CREATE PROCEDURE [dbo].[SelectUsersByFilter]
@adminUserId INT=0, @from INT=0, @size INT=0, @getTemplatesData BINARY=0,@userId INT=0,@userGroupId INT=0, @UserCode INT = 0, @WithPicture BIT=0,@FilterText NVARCHAR (100)=NULL,@Type INT=NULL,@isAdmin BIT=0,@PageNumber AS INT=0, @PageSize AS INT=0
AS
DECLARE  @HasPaging   BIT;
DECLARE  @Total   INT;
BEGIN
 SET @HasPaging = CASE
                                 WHEN @PageSize =0
                                      AND @PageNumber =0 THEN
                                     0                                   
                                 ELSE
                                     1
                             END;
select  @Total =  COUNT(distinct(t.UniqueId))
        FROM   [dbo].[User] t
        left outer join [dbo].[UserGroupMember] ug  on (t.Id=ug.UserId)
        WHERE  (ISNULL(@adminUserId, 0) = 0
                OR EXISTS (SELECT Id
                           FROM   [dbo].[USER]
                           WHERE  [IsAdmin] = 1
                                  AND [Id] = @adminUserId)
                OR EXISTS (SELECT Id
                           FROM   [dbo].[USER]
                           WHERE  [IsMasterAdmin] = 1
                                  AND [Id] = @adminUserId))
								   AND (t.IsAdmin = @isAdmin
                OR ISNULL(@isAdmin , 0) = 0)
	  AND (t.Id =@UserId 
                OR ISNULL(@UserId , 0) = 0)
	  AND (ug.GroupId =@userGroupId 
                OR ISNULL(@userGroupId , 0) = 0)
	   AND (t.Type=@Type
                OR ISNULL(@Type , 0) = 0)
		AND (t.Code =@UserCode
                OR ISNULL(@UserCode , 0) = 0)
	   AND((@FilterText IS NULL)
       OR (t.FirstName LIKE '%' + @FilterText + '%')
       OR (t.SurName LIKE '%' + @FilterText + '%')
       OR (t.Code LIKE @FilterText + '%'))

 IF @HasPaging = 1
     BEGIN
					  
    IF (@size IS NULL
        OR @size = 0)
        SELECT @size = COUNT(1)
        FROM   [dbo].[User]
        WHERE  (ISNULL(@adminUserId, 0) = 0
                OR EXISTS (SELECT Id
                           FROM   [dbo].[USER]
                           WHERE  [IsAdmin] = 1
                                  AND [Id] = @adminUserId)
                OR EXISTS (SELECT Id
                           FROM   [dbo].[USER]
                           WHERE  [IsMasterAdmin] = 1
                                  AND [Id] = @adminUserId));
 
	WITH   U
    AS     (SELECT DENSE_RANK() OVER (ORDER BY [US].[Id]) AS RowNum,
                   [US].[Id]  AS Data_Id,
				   [US].[Code] AS Data_Code,
				   [US].[UniqueId] AS Data_UniqueId,
                   [US].[FirstName] AS Data_FirstName,
                   [US].[SurName] AS Data_SurName,
                   ISNULL([US].[UserName], [US].[FirstName] + ' ' + [US].[SurName]) AS Data_UserName,
                   CAST ([US].[Code] AS NVARCHAR (20)) + '_' + ISNULL([US].[FirstName] + ' ', '')  + ISNULL([US].[SurName], '') AS Data_FullName,
                   [US].[StartDate] AS Data_StartDate,
                   [US].[EndDate] AS Data_EndDate,
                   [US].[RegisterDate] AS Data_RegisterDate,
                   [US].[IsActive] AS Data_IsActive,
                   [US].[AdminLevel] AS Data_AdminLevel,
                   [US].[Password] AS Data_Password,
                   [US].[AuthMode] AS Data_AuthMode,
                   [US].[Email] AS Data_Email,
                   [US].[TelNumber] AS Data_TelNumber,
                   [US].[Type] AS Data_Type,
                   [US].[EntityId] AS Data_EntityId,
                   [US].[IsAdmin] AS Data_IsAdmin,
                   [US].[IsMasterAdmin] AS Data_IsMasterAdmin,
                   [FP].[Id] AS Data_FingerTemplates_Id,
                   [FP].[Index] AS Data_FingerTemplates_Index,
                   [FP].[TemplateIndex] AS Data_FingerTemplates_TemplateIndex,
                   [FP].[Duress] AS Data_FingerTemplates_Duress,
                   [FP].[EnrollQuality] AS Data_FingerTemplates_EnrollQuality,
                   [FP].[Size] AS Data_FingerTemplates_Size,
                   [FP].[SecurityLevel] AS Data_FingerTemplates_SecurityLevel,
                   [FP].[CheckSum] AS Data_FingerTemplates_CheckSum,
                   [FP].[Template] AS Data_FingerTemplates_Template,
                   [FIL].[Code] AS Data_FingerTemplates_FingerIndex_Code,
                   [FIL].[LookupCategoryId] AS Data_FingerTemplates_FingerIndex_LookupCategoryId,
                   [FIL].[Name] AS Data_FingerTemplates_FingerIndex_Name,
                   [FIL].[OrderIndex] AS Data_FingerTemplates_FingerIndex_OrderIndex,
                   [FIL].[Description] AS Data_FingerTemplates_FingerIndex_Description,
                   [FPL].[Code] AS Data_FingerTemplates_FingerTemplateType_Code,
                   [FPL].[LookupCategoryId] AS Data_FingerTemplates_FingerTemplateType_LookupCategoryId,
                   [FPL].[Name] AS Data_FingerTemplates_FingerTemplateType_Name,
                   [FPL].[OrderIndex] AS Data_FingerTemplates_FingerTemplateType_OrderIndex,
                   [FPL].[Description] AS Data_FingerTemplates_FingerTemplateType_Description,
                   [FT].[Id] AS Data_FaceTemplates_Id,
                   [FT].[Template] AS Data_FaceTemplates_Template,
                   [FT].[Index] AS Data_FaceTemplates_Index,
                   [FT].[EnrollQuality] AS Data_FaceTemplates_EnrollQuality,
                   [FT].[Size] AS Data_FaceTemplates_Size,
                   [FTL].[Code] AS Data_FaceTemplates_FaceTemplateType_Code,
                   [FTL].[LookupCategoryId] AS Data_FaceTemplates_FaceTemplateType_LookupCategoryId,
                   [FTL].[Name] AS Data_FaceTemplates_FaceTemplateType_Name,
                   [FTL].[OrderIndex] AS Data_FaceTemplates_FaceTemplateType_OrderIndex,
                   [FTL].[Description] AS Data_FaceTemplates_FaceTemplateType_Description,
                   [FT].[CheckSum] AS Data_FaceTemplates_CheckSum,
                   [FT].[SecurityLevel] AS Data_FaceTemplates_SecurityLevel,
                   [FT].[CreateBy] AS Data_FaceTemplates_CreateBy,
                   [FT].[CreateAt] AS Data_FaceTemplates_CreateAt,
                   [FT].[UpdateBy] AS Data_FaceTemplates_UpdateBy,
                   [FT].[UpdateAt] AS Data_FaceTemplates_UpdateAt,
				   [IT].[Id] AS Data_IrisTemplates_Id,
                   [IT].[Template] AS Data_IrisTemplates_Template,
                   [IT].[Index] AS Data_IrisTemplates_Index,
                   [IT].[EnrollQuality] AS Data_IrisTemplates_EnrollQuality,
                   [IT].[Size] AS Data_IrisTemplates_Size,
                   [ITL].[Code] AS Data_IrisTemplates_IrisTemplateType_Code,
                   [ITL].[LookupCategoryId] AS Data_IrisTemplates_IrisTemplateType_LookupCategoryId,
                   [ITL].[Name] AS Data_IrisTemplates_IrisTemplateType_Name,
                   [ITL].[OrderIndex] AS Data_IrisTemplates_IrisTemplateType_OrderIndex,
                   [ITL].[Description] AS Data_IrisTemplates_IrisTemplateType_Description,
                   [IT].[CheckSum] AS Data_IrisTemplates_CheckSum,
                   [IT].[SecurityLevel] AS Data_IrisTemplates_SecurityLevel,
                   [IT].[CreateBy] AS Data_IrisTemplates_CreateBy,
                   [IT].[CreateAt] AS Data_IrisTemplates_CreateAt,
                   [IT].[UpdateBy] AS Data_IrisTemplates_UpdateBy,
                   [IT].[UpdateAt] AS Data_IrisTemplates_UpdateAt,
				   [UC].[Id] AS Data_IdentityCard_Id,
                   [UC].[CardNum] AS Data_IdentityCard_Number,
                   [UC].[DataCheck] AS Data_IdentityCard_DataCheck,
                   [UC].[IsActive] AS Data_IdentityCard_IsActive,
                   [UC].[IsDeleted] AS Data_IdentityCard_IsDeleted,
                   [ug].[GroupId]  AS Data_GroupId
            FROM   [dbo].[User] AS [US]
                   left outer join [dbo].[UserGroupMember] [ug]  on ([US].[Id]=[ug].[UserId])
                   LEFT OUTER JOIN
                   [dbo].[FingerTemplate] AS [FP]
                   ON [FP].[UserId] = [US].[Id]
                   LEFT OUTER JOIN
                   [dbo].[FaceTemplate] AS [FT]
                   ON [FT].[UserId] = [US].[Id]
				   LEFT OUTER JOIN
                   [dbo].[IrisTemplate] AS [IT]
                   ON [IT].[UserId] = [US].[Id]
                   LEFT OUTER JOIN
                   [dbo].[UserCard] AS [UC]
                   ON US.[Id] = [UC].[UserId] and isnull([UC].IsActive,0)=1
                   LEFT OUTER JOIN
                   [dbo].[Lookup] AS [FIL]
                   ON [FP].[FingerIndex] = [FIL].[Code]
                   LEFT OUTER JOIN
                   [dbo].[Lookup] AS [FPL]
                   ON [FP].[FingerTemplateType] = [FPL].[Code]
                   LEFT OUTER JOIN
                   [dbo].[Lookup] AS [FTL]
                   ON [FT].[FaceTemplateType] = [FTL].[Code]
				   LEFT OUTER JOIN
                   [dbo].[Lookup] AS [ITL]
                   ON [IT].[IrisTemplateType] = [ITL].[Code]
            WHERE  (ISNULL(@adminUserId, 0) = 0
                    OR EXISTS (SELECT top 1 1
                               FROM   [dbo].[USER]
                               WHERE  [IsAdmin] = 1
                                      AND [Id] = @adminUserId)
                    OR EXISTS (SELECT top 1 1
                               FROM   [dbo].[USER]
                               WHERE  [IsMasterAdmin] = 1
                                      AND [Id] = @adminUserId))
                   --AND ISNULL([UC].[IsActive], 1) = 1
                   
                   AND ([US].[IsAdmin] = @isAdmin  OR ISNULL(@isAdmin , 0) = 0)
	               AND ([US].[Id] =@UserId  OR ISNULL(@UserId , 0) = 0)
	               AND ([ug].[GroupId] =@userGroupId  OR ISNULL(@userGroupId , 0) = 0)
	               AND ([US].[Type] =@Type OR ISNULL(@Type , 0) = 0)
		           AND ([US].[Code] =@UserCode  OR ISNULL(@UserCode , 0) = 0)
	               AND((@FilterText IS NULL) OR ([US].[FirstName] LIKE '%' + @FilterText + '%') OR ([US].[SurName] LIKE '%' + @FilterText + '%') OR ([US].[Code] LIKE @FilterText + '%'))
        ) 
    SELECT [Data_Id],
	       [Data_Code],
           [Data_UniqueId],
           [Data_FirstName],
           [Data_SurName],
           [Data_UserName],
           [Data_FullName],
           [Data_StartDate],
           [Data_EndDate],
           [Data_RegisterDate],
           [Data_IsActive],
           [Data_AdminLevel],
           [Data_Password],
           [Data_AuthMode],
           [Data_Email],
           [Data_TelNumber],
           [Data_Type],
           [Data_EntityId],
           [Data_IsAdmin],
           [Data_IsMasterAdmin],
           [Data_FingerTemplates_Id],
           [Data_FingerTemplates_Index],
           [Data_FingerTemplates_TemplateIndex],
           [Data_FingerTemplates_Duress],
           [Data_FingerTemplates_EnrollQuality],
           [Data_FingerTemplates_Size],
           [Data_FingerTemplates_SecurityLevel],
           [Data_FingerTemplates_CheckSum],
           CASE WHEN @getTemplatesData = 1 THEN [Data_FingerTemplates_Template] ELSE NULL END AS [Data_FingerTemplates_Template],
           [Data_FingerTemplates_FingerIndex_Code],
           [Data_FingerTemplates_FingerIndex_LookupCategoryId],
           [Data_FingerTemplates_FingerIndex_Name],
           [Data_FingerTemplates_FingerIndex_OrderIndex],
           [Data_FingerTemplates_FingerIndex_Description],
           [Data_FingerTemplates_FingerTemplateType_Code],
           [Data_FingerTemplates_FingerTemplateType_LookupCategoryId],
           [Data_FingerTemplates_FingerTemplateType_Name],
           [Data_FingerTemplates_FingerTemplateType_OrderIndex],
           [Data_FingerTemplates_FingerTemplateType_Description],
           [Data_FaceTemplates_Id],
           [Data_FaceTemplates_Index],
           [Data_FaceTemplates_EnrollQuality],
           [Data_FaceTemplates_Size],
           CASE WHEN @getTemplatesData = 1 THEN [Data_FaceTemplates_Template] ELSE NULL END AS [Data_FaceTemplates_Template],
           [Data_FaceTemplates_FaceTemplateType_Code],
           [Data_FaceTemplates_FaceTemplateType_LookupCategoryId],
           [Data_FaceTemplates_FaceTemplateType_Name],
           [Data_FaceTemplates_FaceTemplateType_OrderIndex],
           [Data_FaceTemplates_FaceTemplateType_Description],
           [Data_FaceTemplates_CheckSum],
           [Data_FaceTemplates_SecurityLevel],
           [Data_FaceTemplates_CreateBy],
           [Data_FaceTemplates_CreateAt],
           [Data_FaceTemplates_UpdateBy],
           [Data_FaceTemplates_UpdateAt],
		   [Data_IrisTemplates_Id],
           [Data_IrisTemplates_Index],
           [Data_IrisTemplates_EnrollQuality],
           [Data_IrisTemplates_Size],
           CASE WHEN @getTemplatesData = 1 THEN [Data_IrisTemplates_Template] ELSE NULL END AS [Data_IrisTemplates_Template],
           [Data_IrisTemplates_IrisTemplateType_Code],
           [Data_IrisTemplates_IrisTemplateType_LookupCategoryId],
           [Data_IrisTemplates_IrisTemplateType_Name],
           [Data_IrisTemplates_IrisTemplateType_OrderIndex],
           [Data_IrisTemplates_IrisTemplateType_Description],
           [Data_IrisTemplates_CheckSum],
           [Data_IrisTemplates_SecurityLevel],
           [Data_IrisTemplates_CreateBy],
           [Data_IrisTemplates_CreateAt],
           [Data_IrisTemplates_UpdateBy],
           [Data_IrisTemplates_UpdateAt],
           [Data_IdentityCard_Id],
           [Data_IdentityCard_Number],
           [Data_IdentityCard_DataCheck],
           [Data_IdentityCard_IsActive],
           [Data_IdentityCard_IsDeleted],
		   (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   @Total AS [Count]
    FROM   U
    WHERE  RowNum >= ISNULL(@from, 1)
           AND RowNum <= ISNULL(@from, 0) + ISNULL(@size, 0)
	   ORDER BY U.Data_Id
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY	
   END
   Else
BEGIN
 IF (@size IS NULL
        OR @size = 0)
        SELECT @size = COUNT(1)
        FROM   [dbo].[User]
        WHERE  (ISNULL(@adminUserId, 0) = 0
                OR EXISTS (SELECT Id
                           FROM   [dbo].[USER]
                           WHERE  [IsAdmin] = 1
                                  AND [Id] = @adminUserId)
                OR EXISTS (SELECT Id
                           FROM   [dbo].[USER]
                           WHERE  [IsMasterAdmin] = 1
                                  AND [Id] = @adminUserId));
    WITH   U
    AS     (SELECT DENSE_RANK() OVER (ORDER BY [US].[Id]) AS RowNum,
                   [US].[Id]  AS Data_Id,
				   [US].[Code] AS Data_Code,
				   [US].[UniqueId] AS Data_UniqueId,
                   [US].[FirstName] AS Data_FirstName,
                   [US].[SurName] AS Data_SurName,
                   ISNULL([US].[UserName], [US].[FirstName] + ' ' + [US].[SurName]) AS Data_UserName,
                   CAST ([US].[Code] AS NVARCHAR (20)) + '_' + ISNULL([US].[FirstName] + ' ', '')  + ISNULL([US].[SurName], '') AS Data_FullName,
                   [US].[StartDate] AS Data_StartDate,
                   [US].[EndDate] AS Data_EndDate,
                   [US].[RegisterDate] AS Data_RegisterDate,
                   [US].[IsActive] AS Data_IsActive,
                   [US].[AdminLevel] AS Data_AdminLevel,
                   [US].[Password] AS Data_Password,
                   [US].[AuthMode] AS Data_AuthMode,
                   [US].[Email] AS Data_Email,
                   [US].[TelNumber] AS Data_TelNumber,
                   [US].[Type] AS Data_Type,
                   [US].[EntityId] AS Data_EntityId,
                   [US].[IsAdmin] AS Data_IsAdmin,
                   [US].[IsMasterAdmin] AS Data_IsMasterAdmin,
                   [FP].[Id] AS Data_FingerTemplates_Id,
                   [FP].[Index] AS Data_FingerTemplates_Index,
                   [FP].[TemplateIndex] AS Data_FingerTemplates_TemplateIndex,
                   [FP].[Duress] AS Data_FingerTemplates_Duress,
                   [FP].[EnrollQuality] AS Data_FingerTemplates_EnrollQuality,
                   [FP].[Size] AS Data_FingerTemplates_Size,
                   [FP].[SecurityLevel] AS Data_FingerTemplates_SecurityLevel,
                   [FP].[CheckSum] AS Data_FingerTemplates_CheckSum,
                   [FP].[Template] AS Data_FingerTemplates_Template,
                   [FIL].[Code] AS Data_FingerTemplates_FingerIndex_Code,
                   [FIL].[LookupCategoryId] AS Data_FingerTemplates_FingerIndex_LookupCategoryId,
                   [FIL].[Name] AS Data_FingerTemplates_FingerIndex_Name,
                   [FIL].[OrderIndex] AS Data_FingerTemplates_FingerIndex_OrderIndex,
                   [FIL].[Description] AS Data_FingerTemplates_FingerIndex_Description,
                   [FPL].[Code] AS Data_FingerTemplates_FingerTemplateType_Code,
                   [FPL].[LookupCategoryId] AS Data_FingerTemplates_FingerTemplateType_LookupCategoryId,
                   [FPL].[Name] AS Data_FingerTemplates_FingerTemplateType_Name,
                   [FPL].[OrderIndex] AS Data_FingerTemplates_FingerTemplateType_OrderIndex,
                   [FPL].[Description] AS Data_FingerTemplates_FingerTemplateType_Description,
                   [FT].[Id] AS Data_FaceTemplates_Id,
                   [FT].[Template] AS Data_FaceTemplates_Template,
                   [FT].[Index] AS Data_FaceTemplates_Index,
                   [FT].[EnrollQuality] AS Data_FaceTemplates_EnrollQuality,
                   [FT].[Size] AS Data_FaceTemplates_Size,
                   [FTL].[Code] AS Data_FaceTemplates_FaceTemplateType_Code,
                   [FTL].[LookupCategoryId] AS Data_FaceTemplates_FaceTemplateType_LookupCategoryId,
                   [FTL].[Name] AS Data_FaceTemplates_FaceTemplateType_Name,
                   [FTL].[OrderIndex] AS Data_FaceTemplates_FaceTemplateType_OrderIndex,
                   [FTL].[Description] AS Data_FaceTemplates_FaceTemplateType_Description,
                   [FT].[CheckSum] AS Data_FaceTemplates_CheckSum,
                   [FT].[SecurityLevel] AS Data_FaceTemplates_SecurityLevel,
                   [FT].[CreateBy] AS Data_FaceTemplates_CreateBy,
                   [FT].[CreateAt] AS Data_FaceTemplates_CreateAt,
                   [FT].[UpdateBy] AS Data_FaceTemplates_UpdateBy,
                   [FT].[UpdateAt] AS Data_FaceTemplates_UpdateAt,
				   [IT].[Id] AS Data_IrisTemplates_Id,
                   [IT].[Template] AS Data_IrisTemplates_Template,
                   [IT].[Index] AS Data_IrisTemplates_Index,
                   [IT].[EnrollQuality] AS Data_IrisTemplates_EnrollQuality,
                   [IT].[Size] AS Data_IrisTemplates_Size,
                   [ITL].[Code] AS Data_IrisTemplates_IrisTemplateType_Code,
                   [ITL].[LookupCategoryId] AS Data_IrisTemplates_IrisTemplateType_LookupCategoryId,
                   [ITL].[Name] AS Data_IrisTemplates_IrisTemplateType_Name,
                   [ITL].[OrderIndex] AS Data_IrisTemplates_IrisTemplateType_OrderIndex,
                   [ITL].[Description] AS Data_IrisTemplates_IrisTemplateType_Description,
                   [IT].[CheckSum] AS Data_IrisTemplates_CheckSum,
                   [IT].[SecurityLevel] AS Data_IrisTemplates_SecurityLevel,
                   [IT].[CreateBy] AS Data_IrisTemplates_CreateBy,
                   [IT].[CreateAt] AS Data_IrisTemplates_CreateAt,
                   [IT].[UpdateBy] AS Data_IrisTemplates_UpdateBy,
                   [IT].[UpdateAt] AS Data_IrisTemplates_UpdateAt,
                   [UC].[Id] AS Data_IdentityCard_Id,
                   [UC].[CardNum] AS Data_IdentityCard_Number,
                   [UC].[DataCheck] AS Data_IdentityCard_DataCheck,
                   [UC].[IsActive] AS Data_IdentityCard_IsActive,
                   [UC].[IsDeleted] AS Data_IdentityCard_IsDeleted,
                   [ug].[GroupId]  AS Data_GroupId
            FROM   [dbo].[User] AS [US]
                   left outer join [dbo].[UserGroupMember] [ug]  on ([US].[Id]=[ug].[UserId])
                   LEFT OUTER JOIN
                   [dbo].[FingerTemplate] AS [FP]
                   ON [FP].[UserId] = [US].[Id]
                   LEFT OUTER JOIN
                   [dbo].[FaceTemplate] AS [FT]
                   ON [FT].[UserId] = [US].[Id]
                   LEFT OUTER JOIN
                   [dbo].[IrisTemplate] AS [IT]
                   ON [IT].[UserId] = [US].[Id]
                   LEFT OUTER JOIN
                   [dbo].[UserCard] AS [UC]
                   ON US.[Id] = [UC].[UserId] and isnull([UC].IsActive,0)=1
                   LEFT OUTER JOIN
                   [dbo].[Lookup] AS [FIL]
                   ON [FP].[FingerIndex] = [FIL].[Code]
                   LEFT OUTER JOIN
                   [dbo].[Lookup] AS [FPL]
                   ON [FP].[FingerTemplateType] = [FPL].[Code]
                   LEFT OUTER JOIN
                   [dbo].[Lookup] AS [FTL]
                   ON [FT].[FaceTemplateType] = [FTL].[Code]
				   LEFT OUTER JOIN
                   [dbo].[Lookup] AS [ITL]
                   ON [IT].[IrisTemplateType] = [ITL].[Code]
            WHERE  (ISNULL(@adminUserId, 0) = 0
                    OR EXISTS (SELECT Id
                               FROM   [dbo].[USER]
                               WHERE  [IsAdmin] = 1
                                      AND [Id] = @adminUserId)
                    OR EXISTS (SELECT Id
                               FROM   [dbo].[USER]
                               WHERE  [IsMasterAdmin] = 1
                                      AND [Id] = @adminUserId))
                   --AND ISNULL([UC].[IsActive], 1) = 1
                   AND ([US].[IsAdmin] = @isAdmin  OR ISNULL(@isAdmin , 0) = 0)
	               AND ([US].[Id] =@UserId  OR ISNULL(@UserId , 0) = 0)
	               AND ([ug].[GroupId] =@userGroupId  OR ISNULL(@userGroupId , 0) = 0)
	               AND ([US].[Type] =@Type OR ISNULL(@Type , 0) = 0)
		           AND ([US].[Code] =@UserCode  OR ISNULL(@UserCode , 0) = 0)
	               AND((@FilterText IS NULL) OR ([US].[FirstName] LIKE '%' + @FilterText + '%') OR ([US].[SurName] LIKE '%' + @FilterText + '%') OR ([US].[Code] LIKE @FilterText + '%'))
                   ) 
    SELECT [Data_Id],
	       [Data_Code],
		   [Data_UniqueId],
           [Data_FirstName],
           [Data_SurName],
           [Data_UserName],
           [Data_FullName],
           [Data_StartDate],
           [Data_EndDate],
           [Data_RegisterDate],
           [Data_IsActive],
           [Data_AdminLevel],
           [Data_Password],
           [Data_AuthMode],
           [Data_Email],
           [Data_TelNumber],
           [Data_Type],
           [Data_EntityId],
           [Data_IsAdmin],
           [Data_IsMasterAdmin],
           [Data_FingerTemplates_Id],
           [Data_FingerTemplates_Index],
           [Data_FingerTemplates_TemplateIndex],
           [Data_FingerTemplates_Duress],
           [Data_FingerTemplates_EnrollQuality],
           [Data_FingerTemplates_Size],
           [Data_FingerTemplates_SecurityLevel],
           [Data_FingerTemplates_CheckSum],
           CASE WHEN @getTemplatesData = 1 THEN [Data_FingerTemplates_Template] ELSE NULL END AS [Data_FingerTemplates_Template],
           [Data_FingerTemplates_FingerIndex_Code],
           [Data_FingerTemplates_FingerIndex_LookupCategoryId],
           [Data_FingerTemplates_FingerIndex_Name],
           [Data_FingerTemplates_FingerIndex_OrderIndex],
           [Data_FingerTemplates_FingerIndex_Description],
           [Data_FingerTemplates_FingerTemplateType_Code],
           [Data_FingerTemplates_FingerTemplateType_LookupCategoryId],
           [Data_FingerTemplates_FingerTemplateType_Name],
           [Data_FingerTemplates_FingerTemplateType_OrderIndex],
           [Data_FingerTemplates_FingerTemplateType_Description],
           [Data_FaceTemplates_Id],
           [Data_FaceTemplates_Index],
           [Data_FaceTemplates_EnrollQuality],
           [Data_FaceTemplates_Size],
           CASE WHEN @getTemplatesData = 1 THEN [Data_FaceTemplates_Template] ELSE NULL END AS [Data_FaceTemplates_Template],
           [Data_FaceTemplates_FaceTemplateType_Code],
           [Data_FaceTemplates_FaceTemplateType_LookupCategoryId],
           [Data_FaceTemplates_FaceTemplateType_Name],
           [Data_FaceTemplates_FaceTemplateType_OrderIndex],
           [Data_FaceTemplates_FaceTemplateType_Description],
           [Data_FaceTemplates_CheckSum],
           [Data_FaceTemplates_SecurityLevel],
           [Data_FaceTemplates_CreateBy],
           [Data_FaceTemplates_CreateAt],
           [Data_FaceTemplates_UpdateBy],
           [Data_FaceTemplates_UpdateAt],
		   [Data_IrisTemplates_Id],
           [Data_IrisTemplates_Index],
           [Data_IrisTemplates_EnrollQuality],
           [Data_IrisTemplates_Size],
           CASE WHEN @getTemplatesData = 1 THEN [Data_IrisTemplates_Template] ELSE NULL END AS [Data_IrisTemplates_Template],
           [Data_IrisTemplates_IrisTemplateType_Code],
           [Data_IrisTemplates_IrisTemplateType_LookupCategoryId],
           [Data_IrisTemplates_IrisTemplateType_Name],
           [Data_IrisTemplates_IrisTemplateType_OrderIndex],
           [Data_IrisTemplates_IrisTemplateType_Description],
           [Data_IrisTemplates_CheckSum],
           [Data_IrisTemplates_SecurityLevel],
           [Data_IrisTemplates_CreateBy],
           [Data_IrisTemplates_CreateAt],
           [Data_IrisTemplates_UpdateBy],
           [Data_IrisTemplates_UpdateAt],
           [Data_IdentityCard_Id],
           [Data_IdentityCard_Number],
           [Data_IdentityCard_DataCheck],
           [Data_IdentityCard_IsActive],
           [Data_IdentityCard_IsDeleted],
		   1  AS [from],
		   1 AS PageNumber,
		   count(*) OVER() As PageSize,
		   @Total AS [Count]
    FROM   U
    WHERE  RowNum >= ISNULL(@from, 1)
           AND RowNum <= ISNULL(@from, 0) + ISNULL(@size, 0)
END
END