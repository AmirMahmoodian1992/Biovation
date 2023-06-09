Create PROCEDURE [dbo].[SelectTemplatesCount]
AS
BEGIN
 --   SELECT   UserId,
 --            COUNT(*) AS FingerTemplateCount
 --   into #Finger FROM     FingerTemplate
 --   WHERE    (TemplateIndex = 1
 --             AND FingerTemplateType = 11)
 --            OR FingerTemplateType = 10 OR FingerTemplateType = 31
 --   GROUP BY UserId
    
 --   SELECT   UserId,
 --            COUNT(*) AS FaceTemplateCount
 --   into #Face FROM     FaceTemplate
 --   GROUP BY UserId
    
    
 --   SELECT   UserId, COUNT(*) AS CardCount
 --   into #Card FROM     dbo.UserCard   
 --   WHERE IsActive = 1 
 --   GROUP BY UserId;
    
 --   SELECT 
	--case when finger.UserId is null then case when f.userid is null then c.userid else f.userid end else finger.userid end as 'UserId',
	--c.CardCount,
	--f.faceTemplateCount,
	--finger.FingerTemplateCount
	--FROM  #Finger finger left JOIN 
	--#Face f ON f.UserId = finger.UserId
 --   Left JOIN #Card c ON c.UserId = finger.UserID
       
 --       DROP TABLE #finger,#face,#card
     select userId,Max(FingerTemplateCount) as 'FingerTemplateCount',Max(FaceTemplateCount)as 'FaceTemplateCount',Max(CardCount)as 'CardCount'	 from (
	SELECT  UserId,
    COUNT(*) AS FingerTemplateCount,0 as 'FaceTemplateCount',0 as 'CardCount'	
    --into #Finger
	FROM     FingerTemplate
    WHERE    (TemplateIndex = 1
              AND FingerTemplateType = '18001')
             OR FingerTemplateType = '18002' OR FingerTemplateType = '18003'
    GROUP BY UserId
    union 
    SELECT   UserId,
             0 as 'FingerTemplateCount',COUNT(*) AS FaceTemplateCount,0 as 'CardCount'
   -- into #Face
   FROM     FaceTemplate
    GROUP BY UserId    
    union
    SELECT   UserId,  0 as 'FingerTemplateCount',0 as 'FaceTemplateCount',COUNT(*) AS CardCount
    --into #Card
	FROM     dbo.UserCard   
    WHERE IsActive = 1 
    GROUP BY UserId
    )a
  group by UserId

END