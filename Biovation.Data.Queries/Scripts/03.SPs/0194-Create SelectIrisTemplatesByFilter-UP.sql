Create PROCEDURE [dbo].[SelectIrisTemplatesByFilter]
@UserId BIGINT=NULL, @Index INT=NULL,@IrisTemplateType NVARCHAR (50)=NULL,@PageNumber AS INT=NULL,@PageSize AS INT =NULL
AS
     DECLARE  @HasPaging  BIT;
     DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200';
BEGIN

	 SET @HasPaging = CASE
                                       WHEN @PageSize =0
                                      AND @PageNumber =0 THEN
                                     0                                   
                                 ELSE
                                     1
                             END;
 IF @HasPaging = 1
  BEGIN
    SELECT [IT].[Id] AS Data_Id,
           [IT].[UserId] AS Data_UserId,
           [IT].[Template] AS Data_Template,
           [IT].[Index] AS Data_Index,
           [IT].[CheckSum] AS Data_CheckSum,
           [IT].[SecurityLevel] AS Data_SecurityLevel,
           [IT].[EnrollQuality] AS Data_EnrollQuality,
           [IT].[Size] AS Data_Size,
           [ITT].[Code] AS  Data_IrisTemplateType_Code,
           [ITT].[LookupCategoryId] AS  Data_IrisTemplateType_LookupCategoryId,
           [ITT].[Name] AS  Data_IrisTemplateType_Name,
           [ITT].[OrderIndex] AS  Data_IrisTemplateType_OrderIndex,
           [ITT].[Description] AS  Data_IrisTemplateType_Description,
           (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(*) OVER() AS [Count],
           @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
    FROM   [dbo].[IrisTemplate] AS [IT]
           LEFT OUTER JOIN
           [dbo].[Lookup] AS [ITT]
           ON [IT].[IrisTemplateType] = [ITT].[Code]
		   
	WHERE	(UserId = @UserId And ([Index] = @Index
                OR ISNULL(@Index, 0) = 0)
                OR ISNULL(@UserId, 0) = 0 )
				AND ([IT].[IrisTemplateType] = @IrisTemplateType
                OR ISNULL(@IrisTemplateType,'') = '')
			ORDER BY  [IT].[Id]
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY
END
Else
BEGIN
    SELECT [IT].[Id] AS Data_Id,
           [IT].[UserId] AS Data_UserId,
           [IT].[Template] AS Data_Template,
           [IT].[Index] AS Data_Index,
           [IT].[CheckSum] AS Data_CheckSum,
           [IT].[SecurityLevel] AS Data_SecurityLevel,
           [IT].[EnrollQuality] AS Data_EnrollQuality,
           [IT].[Size] AS Data_Size,
           [ITT].[Code] AS  Data_IrisTemplateType_Code,
           [ITT].[LookupCategoryId] AS  Data_IrisTemplateType_LookupCategoryId,
           [ITT].[Name] AS  Data_IrisTemplateType_Name,
           [ITT].[OrderIndex] AS  Data_IrisTemplateType_OrderIndex,
           [ITT].[Description] AS  Data_IrisTemplateType_Description,
           1  AS [from],
		   1 AS PageNumber,
		   count(*) OVER() As PageSize,
		   count(*) OVER() AS [Count],
           @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
    FROM   [dbo].[IrisTemplate] AS [IT]
           LEFT OUTER JOIN
           [dbo].[Lookup] AS [ITT]
           ON [IT].[IrisTemplateType] = [ITT].[Code]
		   
	WHERE	(UserId = @UserId And ([Index] = @Index
                OR ISNULL(@Index, 0) = 0)
                OR ISNULL(@UserId, 0) = 0 )
				AND ([IT].[IrisTemplateType] = @IrisTemplateType
                OR ISNULL(@IrisTemplateType,'') = '') ;
END
END