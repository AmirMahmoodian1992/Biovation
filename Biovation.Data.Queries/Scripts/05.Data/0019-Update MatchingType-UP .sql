IF EXISTS (SELECT MatchingType FROM dbo.[Log] WHERE  [MatchingType]= 3)
    UPDATE dbo.[Log] SET [MatchingType] = '19001' WHERE [MatchingType]= 3

IF EXISTS (SELECT MatchingType FROM dbo.[Log] WHERE  [MatchingType]= 1)
    UPDATE dbo.[Log] SET [MatchingType] = '19002' WHERE [MatchingType]= 1

IF EXISTS (SELECT MatchingType FROM dbo.[Log] WHERE  [MatchingType]= 2)
    UPDATE dbo.[Log] SET [MatchingType] = '19004' WHERE [MatchingType]= 2

IF EXISTS (SELECT MatchingType FROM dbo.[Log] WHERE  [MatchingType]= 15)
    UPDATE dbo.[Log] SET [MatchingType] = '19001' WHERE [MatchingType]= 15

IF EXISTS (SELECT MatchingType FROM dbo.[Log] WHERE  [MatchingType]= 0)
    UPDATE dbo.[Log] SET [MatchingType] = '19002' WHERE [MatchingType]= 0