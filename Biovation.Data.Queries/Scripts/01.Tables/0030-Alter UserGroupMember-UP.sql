IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[UserGroupMember]') AND type in (N'U'))
Begin
IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'CreateDate'
          AND Object_ID = Object_ID(N'[UserGroupMember]'))
		  BEGIN
			ALTER TABLE [UserGroupMember] ADD CreateDate DATETIME
		  END
	
End