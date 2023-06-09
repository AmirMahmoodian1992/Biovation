CREATE PROCEDURE [dbo].[SelectUserByCardNumber]
@CardNumber NVARCHAR (50)
AS
DECLARE @Message AS NVARCHAR (200) = N' درخواست با موفقیت انجام گرفت', @Validate AS INT = 1,  @Code AS NVARCHAR (15) = N'200';

BEGIN
    SELECT [US].[Id],
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
           --[US].[Image],
           [US].[AuthMode],
		   [US].[Email],
		   [US].[TelNumber],
		   [US].[Type],
		   [US].[EntityId],
		   [US].[IsAdmin],
		   [US].[IsMasterAdmin],[FP].[Id] AS FingerTemplates_Id,
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

		   ,@Message AS e_Message,
           @Validate AS e_Validate,
           @Code AS e_Code

    FROM   [dbo].[User] AS [US] 
		   LEFT JOIN 
		   [dbo].[FingerTemplate] AS [FP] ON [FP].[UserId] = [US].[Id]
		   LEFT JOIN
		   [dbo].[FaceTemplate] AS [FT] ON [FT].[UserId] = [US].[Id]
           LEFT JOIN
           [dbo].[UserCard] AS [UC] ON US.[Id] = [UC].[UserId]
		   LEFT JOIN
		   [dbo].[Lookup] AS [FIL] ON [FP].[FingerIndex] = [FIL].[Code]
		   LEFT JOIN
		   [dbo].[Lookup] AS [FPL] ON [FP].[FingerTemplateType] = [FPL].[Code]
		   LEFT JOIN
		   [dbo].[Lookup] AS [FTL] ON [FT].[FaceTemplateType] = [FTL].[Code]
    WHERE  [UC].[IsActive] = 1
           AND [UC].[CardNum] = @CardNumber;
END
GO
