
IF COL_LENGTH('dbo.FingerTemplate', 'FingerTemplateType') IS NULL
BEGIN
    ALTER TABLE dbo.[FingerTemplate]
ADD [FingerTemplateType] int NULL

END
GO

ALTER TABLE [dbo].[FingerTemplate]  WITH CHECK ADD  CONSTRAINT [FK_FingerTemplate_FingerTemplateType] FOREIGN KEY([FingerTemplateType])
REFERENCES [dbo].[FingerTemplateType] ([Id])
GO
