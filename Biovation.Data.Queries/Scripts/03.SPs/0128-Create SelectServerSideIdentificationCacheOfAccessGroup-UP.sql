
CREATE PROCEDURE [dbo].[SelectServerSideIdentificationCacheOfAccessGroup]
@adminUserId INT=0,
@brandCode nvarchar(50),
@AccessGroupId Int
AS
BEGIN
    SELECT ROW_NUMBER() OVER (ORDER BY [AG].[Id] ASC) AS Id,
		   [AG].[Id] AS AccessGroupId,
           [UGM].[UserId] AS UserId,
           [UGM].[UserType] AS UserType,
           [DGM].[DeviceId] AS DeviceId,
		   [Dev].[Code] AS DeviceCode,
		   [FPT].[Id] AS FingerTemplate_Id,
		   [FPT].[UserId] AS FingerTemplate_UserId,
		   [FPT].[TemplateIndex] AS FingerTemplate_TemplateIndex,
           [FPT].[Template] AS FingerTemplate_Template,
           [FPT].[Index] AS FingerTemplate_Index,
           [FPT].[Duress] AS FingerTemplate_Duress,
           [FPT].[CheckSum] AS FingerTemplate_CheckSum,
           [FPT].[SecurityLevel] AS FingerTemplate_SecurityLevel,
           [FPT].[EnrollQuality] AS FingerTemplate_EnrollQuality,
           [FPT].[Size] AS FingerTemplate_Size,
		   [FTL].[Code] AS FingerTemplate_FingerTemplateType_Code,
		   [FTL].[LookupCategoryId] AS FingerTemplate_FingerTemplateType_LookupCategoryId,
		   [FTL].[Name] AS FingerTemplate_FingerTemplateType_Name,
		   [FTL].[OrderIndex] AS FingerTemplate_FingerTemplateType_OrderIndex,
		   [FTL].[Description] AS FingerTemplate_FingerTemplateType_Description,
		   [FIL].[Code] AS FingerTemplate_FingerIndex_Code,
		   [FIL].[LookupCategoryId] AS FingerTemplate_FingerIndex_LookupCategoryId,
		   [FIL].[Name] AS FingerTemplate_FingerIndex_Name,
		   [FIL].[OrderIndex] AS FingerTemplate_FingerIndex_OrderIndex,
		   [FIL].[Description] AS FingerTemplate_FingerIndex_Description
    FROM   [dbo].[AccessGroup] AS AG
           LEFT OUTER JOIN
           [dbo].[AccessGroupUser] AS AGU
           ON AGU.AccessGroupId = AG.ID
           LEFT OUTER JOIN
           [dbo].[UserGroup] AS USG
           ON AGU.UserGroupId = USG.ID
           LEFT OUTER JOIN
           [dbo].[AccessGroupDevice] AS AGD
           ON AGD.AccessGroupId = AG.Id
           LEFT OUTER JOIN
           [dbo].[DeviceGroup] AS DG
           ON AGD.DeviceGroupId = DG.ID
           LEFT OUTER JOIN
           [dbo].[AdminAccessGroup] AS AAG
           ON AG.Id = AAG.AccessGroupId
           LEFT OUTER JOIN
           [dbo].[UserGroupMember] AS UGM
           ON UGM.GroupId = USG.Id
           LEFT OUTER JOIN
           [dbo].[DeviceGroupMember] AS DGM
           ON DGM.GroupId = DG.ID
		   INNER JOIN
		   [dbo].[Device] AS [Dev]
		   ON [DGM].[DeviceId] = [Dev].[Id]
		   INNER JOIN
		   [dbo].[DeviceModel] AS [DM]
		   ON [Dev].[DeviceModelId] = [DM].[Id]
		   INNER JOIN
		   [dbo].[FingerTemplate] AS FPT
		   ON FPT.UserId = UGM.UserId
		   LEFT JOIN
		   [dbo].[Lookup] AS [FIL] 
		   ON [FPT].[FingerIndex] = [FIL].[Code]
		   LEFT JOIN
		   [dbo].[Lookup] AS [FTL] 
		   ON [FPT].[FingerTemplateType] = [FTL].[Code]
		   LEFT JOIN
		   [dbo].[GenericCodeMapping] AS [FTM] 
		   ON [FTM].[GenericCode] = [FPT].[FingerTemplateType]
    WHERE  (AAG.UserId = @adminUserId
           OR ISNULL(@adminUserId, 0) = 0
           OR EXISTS (SELECT Id
                      FROM   [dbo].[USER]
                      WHERE  IsMasterAdmin = 1
                             AND ID = @AdminUserId))

			AND FPT.[CheckSum] <> 0
			--AND FPT.EnrollQuality <> 0
			AND [FTM].[BrandCode] = @brandCode
			AND [DM].[BrandId] = @brandCode
		    AND [AG].[Id] = @AccessGroupId
END