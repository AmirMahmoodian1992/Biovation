Create PROCEDURE [dbo].[SelectFaceTemplatesByFilter]
@UserId BIGINT=NULL, @Index INT=NULL,@FaceTemplateType NVARCHAR (50)=NULL,@PageNumber AS INT=NULL,@PageSize AS INT =NULL
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
    SELECT [FT].[Id] AS Data_Id,
           [FT].[UserId] AS Data_UserId,
           [FT].[Template] AS Data_Template,
           [FT].[Index] AS Data_Index,
           [FT].[CheckSum] AS Data_CheckSum,
           [FT].[SecurityLevel] AS Data_SecurityLevel,
           [FT].[EnrollQuality] AS Data_EnrollQuality,
           [FT].[Size] AS Data_Size,
           [FTLU].[Code] AS  Data_FaceTemplateType_Code,
           [FTLU].[LookupCategoryId] AS  Data_FaceTemplateType_LookupCategoryId,
           [FTLU].[Name] AS  Data_FaceTemplateType_Name,
           [FTLU].[OrderIndex] AS  Data_FaceTemplateType_OrderIndex,
           [FTLU].[Description] AS  Data_FaceTemplateType_Description,
           (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(*) OVER() AS [Count],
           @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
    FROM   [dbo].[FaceTemplate] AS [FT]
           LEFT OUTER JOIN
           [dbo].[Lookup] AS [FTLU]
           ON [FT].[FaceTemplateType] = [FTLU].[Code]
		   
	WHERE	(UserId = @UserId And ([Index] = @Index
                OR ISNULL(@Index, 0) = 0)
                OR ISNULL(@UserId, 0) = 0 )
				AND ([FT].[FaceTemplateType] = @FaceTemplateType
                OR ISNULL(@FaceTemplateType,'') = '')
			ORDER BY  [FT].[Id]
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY
END
Else
BEGIN
    SELECT [FT].[Id] AS Data_Id,
           [FT].[UserId] AS Data_UserId,
           [FT].[Template] AS Data_Template,
           [FT].[Index] AS Data_Index,
           [FT].[CheckSum] AS Data_CheckSum,
           [FT].[SecurityLevel] AS Data_SecurityLevel,
           [FT].[EnrollQuality] AS Data_EnrollQuality,
           [FT].[Size] AS Data_Size,
           [FTLU].[Code] AS  Data_FaceTemplateType_Code,
           [FTLU].[LookupCategoryId] AS  Data_FaceTemplateType_LookupCategoryId,
           [FTLU].[Name] AS  Data_FaceTemplateType_Name,
           [FTLU].[OrderIndex] AS  Data_FaceTemplateType_OrderIndex,
           [FTLU].[Description] AS  Data_FaceTemplateType_Description,
           1  AS [from],
		   1 AS PageNumber,
		   count(*) OVER() As PageSize,
		   count(*) OVER() AS [Count],
           @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
    FROM   [dbo].[FaceTemplate] AS [FT]
           LEFT OUTER JOIN
           [dbo].[Lookup] AS [FTLU]
           ON [FT].[FaceTemplateType] = [FTLU].[Code]
		   
	WHERE	(UserId = @UserId And ([Index] = @Index
                OR ISNULL(@Index, 0) = 0)
                OR ISNULL(@UserId, 0) = 0 )
				AND ([FT].[FaceTemplateType] = @FaceTemplateType
                OR ISNULL(@FaceTemplateType,'') = '') ;
END
END