
CREATE PROCEDURE [dbo].[SelectServerSideIdentificationCacheOfAccessGroup]
@adminUserId INT=0,
@brandCode nvarchar(50),
@AccessGroupId Int, @pageNumber INT = NULL, @PageSize INT = Null
AS
BEGIN
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200';
DECLARE  @HasPaging  BIT;
	 SET @HasPaging = CASE
                                       WHEN @PageSize =0
                                      AND @PageNumber =0 THEN
                                     0                                   
                                 ELSE
                                     1
                             END;
 IF @HasPaging = 1
  BEGIN

    SELECT ROW_NUMBER() OVER (ORDER BY [AG].[Id] ASC) AS Data_Id,
		   [AG].[Id] AS Data_AccessGroupId,
           [UGM].[UserId] AS Data_UserId,
           [UGM].[UserType] AS Data_UserType,
           [DGM].[DeviceId] AS Data_DeviceId,
		   [Dev].[Code] AS Data_DeviceCode,
		   [FPT].[Id] AS Data_FingerTemplate_Id,
		   [FPT].[UserId] AS Data_FingerTemplate_UserId,
		   [FPT].[TemplateIndex] AS Data_FingerTemplate_TemplateIndex,
           [FPT].[Template] AS Data_FingerTemplate_Template,
           [FPT].[Index] AS Data_FingerTemplate_Index,
           [FPT].[Duress] AS Data_FingerTemplate_Duress,
           [FPT].[CheckSum] AS Data_FingerTemplate_CheckSum,
           [FPT].[SecurityLevel] AS Data_FingerTemplate_SecurityLevel,
           [FPT].[EnrollQuality] AS Data_FingerTemplate_EnrollQuality,
           [FPT].[Size] AS Data_FingerTemplate_Size,
		   [FTL].[Code] AS Data_FingerTemplate_FingerTemplateType_Code,
		   [FTL].[LookupCategoryId] AS Data_FingerTemplate_FingerTemplateType_LookupCategoryId,
		   [FTL].[Name] AS Data_FingerTemplate_FingerTemplateType_Name,
		   [FTL].[OrderIndex] AS Data_FingerTemplate_FingerTemplateType_OrderIndex,
		   [FTL].[Description] AS Data_FingerTemplate_FingerTemplateType_Description,
		   [FIL].[Code] AS Data_FingerTemplate_FingerIndex_Code,
		   [FIL].[LookupCategoryId] AS Data_FingerTemplate_FingerIndex_LookupCategoryId,
		   [FIL].[Name] AS Data_FingerTemplate_FingerIndex_Name,
		   [FIL].[OrderIndex] AS Data_FingerTemplate_FingerIndex_OrderIndex,
		   [FIL].[Description] AS Data_FingerTemplate_FingerIndex_Description,
           (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate

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
            ORDER BY dev.Id
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY	

END
ELSE
  BEGIN

     SELECT ROW_NUMBER() OVER (ORDER BY [AG].[Id] ASC) AS Data_Id,
		   [AG].[Id] AS Data_AccessGroupId,
           [UGM].[UserId] AS Data_UserId,
           [UGM].[UserType] AS Data_UserType,
           [DGM].[DeviceId] AS Data_DeviceId,
		   [Dev].[Code] AS Data_DeviceCode,
		   [FPT].[Id] AS Data_FingerTemplate_Id,
		   [FPT].[UserId] AS Data_FingerTemplate_UserId,
		   [FPT].[TemplateIndex] AS Data_FingerTemplate_TemplateIndex,
           [FPT].[Template] AS Data_FingerTemplate_Template,
           [FPT].[Index] AS Data_FingerTemplate_Index,
           [FPT].[Duress] AS Data_FingerTemplate_Duress,
           [FPT].[CheckSum] AS Data_FingerTemplate_CheckSum,
           [FPT].[SecurityLevel] AS Data_FingerTemplate_SecurityLevel,
           [FPT].[EnrollQuality] AS Data_FingerTemplate_EnrollQuality,
           [FPT].[Size] AS Data_FingerTemplate_Size,
		   [FTL].[Code] AS Data_FingerTemplate_FingerTemplateType_Code,
		   [FTL].[LookupCategoryId] AS Data_FingerTemplate_FingerTemplateType_LookupCategoryId,
		   [FTL].[Name] AS Data_FingerTemplate_FingerTemplateType_Name,
		   [FTL].[OrderIndex] AS Data_FingerTemplate_FingerTemplateType_OrderIndex,
		   [FTL].[Description] AS Data_FingerTemplate_FingerTemplateType_Description,
		   [FIL].[Code] AS Data_FingerTemplate_FingerIndex_Code,
		   [FIL].[LookupCategoryId] AS Data_FingerTemplate_FingerIndex_LookupCategoryId,
		   [FIL].[Name] AS Data_FingerTemplate_FingerIndex_Name,
		   [FIL].[OrderIndex] AS Data_FingerTemplate_FingerIndex_OrderIndex,
		   [FIL].[Description] AS Data_FingerTemplate_FingerIndex_Description,
           1 AS [from],
		   1 AS PageNumber,
		   count(*) OVER() As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate

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
END