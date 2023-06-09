CREATE PROCEDURE [dbo].[SelectUsers]
@adminUserId INT = 0, @from int = 0, @size int = 0, @getTemplatesData binary = 0
AS
BEGIN
    IF (@size IS NULL
        OR @size = 0)
        SELECT @size = COUNT(*)
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
                   [US].[Id],
				   [US].[Code],
		           [US].[UniqueId],
                   [US].[FirstName],
                   [US].[SurName],
                   ISNULL([US].[UserName], [US].[FirstName] + ' ' + [US].[SurName]) AS UserName,
                   [US].[StartDate],
                   [US].[EndDate],
                   [US].[RegisterDate],
                   [US].[IsActive],
                   [US].[AdminLevel],
                   [US].[Password],
                   [US].[AuthMode],
                   [US].[Email],
                   [US].[TelNumber],
                   [US].[Type],
                   [US].[EntityId],
                   [US].[IsAdmin],
                   [US].[IsMasterAdmin],
                   [FP].[Id] AS FingerTemplates_Id,
                   [FP].[Index] AS FingerTemplates_Index,
                   [FP].[TemplateIndex] AS FingerTemplates_TemplateIndex,
                   [FP].[Duress] AS FingerTemplates_Duress,
                   [FP].[EnrollQuality] AS FingerTemplates_EnrollQuality,
                   [FP].[Size] AS FingerTemplates_Size,
                   [FP].[SecurityLevel] AS FingerTemplates_SecurityLevel,
                   [FP].[CheckSum] AS FingerTemplates_CheckSum,
                   [FP].[Template] AS FingerTemplates_Template,
                   [FIL].[Code] AS FingerTemplates_FingerIndex_Code,
                   [FIL].[LookupCategoryId] AS FingerTemplates_FingerIndex_LookupCategoryId,
                   [FIL].[Name] AS FingerTemplates_FingerIndex_Name,
                   [FIL].[OrderIndex] AS FingerTemplates_FingerIndex_OrderIndex,
                   [FIL].[Description] AS FingerTemplates_FingerIndex_Description,
                   [FPL].[Code] AS FingerTemplates_FingerTemplateType_Code,
                   [FPL].[LookupCategoryId] AS FingerTemplates_FingerTemplateType_LookupCategoryId,
                   [FPL].[Name] AS FingerTemplates_FingerTemplateType_Name,
                   [FPL].[OrderIndex] AS FingerTemplates_FingerTemplateType_OrderIndex,
                   [FPL].[Description] AS FingerTemplates_FingerTemplateType_Description,
                   [FT].[Id] AS FaceTemplates_Id,
                   [FT].[Template] AS FaceTemplates_Template,
                   [FT].[Index] AS FaceTemplates_Index,
                   [FT].[EnrollQuality] AS FaceTemplates_EnrollQuality,
                   [FT].[Size] AS FaceTemplates_Size,
                   [FTL].[Code] AS FaceTemplates_FaceTemplateType_Code,
                   [FTL].[LookupCategoryId] AS FaceTemplates_FaceTemplateType_LookupCategoryId,
                   [FTL].[Name] AS FaceTemplates_FaceTemplateType_Name,
                   [FTL].[OrderIndex] AS FaceTemplates_FaceTemplateType_OrderIndex,
                   [FTL].[Description] AS FaceTemplates_FaceTemplateType_Description,
                   [FT].[CheckSum] AS FaceTemplates_CheckSum,
                   [FT].[SecurityLevel] AS FaceTemplates_SecurityLevel,
                   [FT].[CreateBy] AS FaceTemplates_CreateBy,
                   [FT].[CreateAt] AS FaceTemplates_CreateAt,
                   [FT].[UpdateBy] AS FaceTemplates_UpdateBy,
                   [FT].[UpdateAt] AS FaceTemplates_UpdateAt,
                   [UC].[Id] AS IdentityCard_Id,
                   [UC].[CardNum] AS IdentityCard_Number,
                   [UC].[DataCheck] AS IdentityCard_DataCheck,
                   [UC].[IsActive] AS IdentityCard_IsActive,
                   [UC].[IsDeleted] AS IdentityCard_IsDeleted
            FROM   [dbo].[User] AS [US]
                   LEFT OUTER JOIN
                   [dbo].[FingerTemplate] AS [FP]
                   ON [FP].[UserId] = [US].[Id]
                   LEFT OUTER JOIN
                   [dbo].[FaceTemplate] AS [FT]
                   ON [FT].[UserId] = [US].[Id]
                   LEFT OUTER JOIN
                   [dbo].[UserCard] AS [UC]
                   ON US.[Id] = [UC].[UserId]
                   LEFT OUTER JOIN
                   [dbo].[Lookup] AS [FIL]
                   ON [FP].[FingerIndex] = [FIL].[Code]
                   LEFT OUTER JOIN
                   [dbo].[Lookup] AS [FPL]
                   ON [FP].[FingerTemplateType] = [FPL].[Code]
                   LEFT OUTER JOIN
                   [dbo].[Lookup] AS [FTL]
                   ON [FT].[FaceTemplateType] = [FTL].[Code]
            WHERE  (ISNULL(@adminUserId, 0) = 0
                    OR EXISTS (SELECT Id
                               FROM   [dbo].[USER]
                               WHERE  [IsAdmin] = 1
                                      AND [Code] = @adminUserId)
                    OR EXISTS (SELECT Id
                               FROM   [dbo].[USER]
                               WHERE  [IsMasterAdmin] = 1
                                      AND [Code] = @adminUserId))
                   --AND ISNULL([UC].[IsActive], 1) = 1
                   )
    SELECT [Id],
		   [Code],
		   [UniqueId],
           [FirstName],
           [SurName],
           [UserName],
           [StartDate],
           [EndDate],
           [RegisterDate],
           [IsActive],
           [AdminLevel],
           [Password],
           [AuthMode],
           [Email],
           [TelNumber],
           [Type],
           [EntityId],
           [IsAdmin],
           [IsMasterAdmin],
           [FingerTemplates_Id],
           [FingerTemplates_Index],
           [FingerTemplates_TemplateIndex],
           [FingerTemplates_Duress],
           [FingerTemplates_EnrollQuality],
           [FingerTemplates_Size],
           [FingerTemplates_SecurityLevel],
           [FingerTemplates_CheckSum],
           CASE WHEN @getTemplatesData = 1 THEN [FingerTemplates_Template] ELSE NULL END AS [FingerTemplates_Template],
           [FingerTemplates_FingerIndex_Code],
           [FingerTemplates_FingerIndex_LookupCategoryId],
           [FingerTemplates_FingerIndex_Name],
           [FingerTemplates_FingerIndex_OrderIndex],
           [FingerTemplates_FingerIndex_Description],
           [FingerTemplates_FingerTemplateType_Code],
           [FingerTemplates_FingerTemplateType_LookupCategoryId],
           [FingerTemplates_FingerTemplateType_Name],
           [FingerTemplates_FingerTemplateType_OrderIndex],
           [FingerTemplates_FingerTemplateType_Description],
           [FaceTemplates_Id],
           [FaceTemplates_Index],
           [FaceTemplates_EnrollQuality],
           [FaceTemplates_Size],
           CASE WHEN @getTemplatesData = 1 THEN [FaceTemplates_Template] ELSE NULL END AS [FaceTemplates_Template],
           [FaceTemplates_FaceTemplateType_Code],
           [FaceTemplates_FaceTemplateType_LookupCategoryId],
           [FaceTemplates_FaceTemplateType_Name],
           [FaceTemplates_FaceTemplateType_OrderIndex],
           [FaceTemplates_FaceTemplateType_Description],
           [FaceTemplates_CheckSum],
           [FaceTemplates_SecurityLevel],
           [FaceTemplates_CreateBy],
           [FaceTemplates_CreateAt],
           [FaceTemplates_UpdateBy],
           [FaceTemplates_UpdateAt],
           [IdentityCard_Id],
           [IdentityCard_Number],
           [IdentityCard_DataCheck],
           [IdentityCard_IsActive],
           [IdentityCard_IsDeleted]
    FROM   U
	WHERE RowNum >= ISNULL(@from, 1)
	  AND RowNum <= ISNULL(@from, 0) + ISNULL(@size, 0)
END
GO
