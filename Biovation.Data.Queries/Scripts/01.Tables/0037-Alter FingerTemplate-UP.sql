IF COL_LENGTH('dbo.FingerTemplate', 'CreateBy') IS NULL
BEGIN
ALTER TABLE dbo.[FingerTemplate]
ADD
[CreateBy] [bigint] NULL
END
IF COL_LENGTH('dbo.FingerTemplate', 'CreateAt') IS NULL
BEGIN
ALTER TABLE dbo.[FingerTemplate]
ADD
[CreateAt] [datetime] NULL
END
IF COL_LENGTH('dbo.FingerTemplate', 'UpdateBy') IS NULL
BEGIN
ALTER TABLE dbo.[FingerTemplate]
ADD
[UpdateBy] [bigint] NULL
END
IF COL_LENGTH('dbo.FingerTemplate', 'UpdateAt') IS NULL
BEGIN
ALTER TABLE dbo.[FingerTemplate]
ADD
[UpdateAt] [datetime] NULL
END

IF COL_LENGTH('dbo.FaceTemplate', 'CreateBy') IS NULL
BEGIN
ALTER TABLE dbo.FaceTemplate
ADD
[CreateBy] [bigint] NULL
END
IF COL_LENGTH('dbo.FaceTemplate', 'CreateAt') IS NULL
BEGIN
ALTER TABLE dbo.FaceTemplate
ADD
[CreateAt] [datetime] NULL
END
IF COL_LENGTH('dbo.FaceTemplate', 'UpdateBy') IS NULL
BEGIN
ALTER TABLE dbo.FaceTemplate
ADD
[UpdateBy] [bigint] NULL
END
IF COL_LENGTH('dbo.FaceTemplate', 'UpdateAt') IS NULL
BEGIN
ALTER TABLE dbo.FaceTemplate
ADD
[UpdateAt] [datetime] NULL
END