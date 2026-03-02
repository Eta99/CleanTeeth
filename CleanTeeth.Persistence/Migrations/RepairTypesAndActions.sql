-- Run this script manually if migration 20260302172430_type failed with FK constraint error.
-- Ensures Types has row Id=1 and all Actions.TypeId are valid, then adds FK if missing.

-- 1) Ensure Types has row with Id = 1
SET IDENTITY_INSERT [dbo].[Types] ON;
IF NOT EXISTS (SELECT 1 FROM [dbo].[Types] WHERE [Id] = 1)
    INSERT INTO [dbo].[Types] ([Id], [Name]) VALUES (1, N'Default');
SET IDENTITY_INSERT [dbo].[Types] OFF;

-- 2) Fix any invalid TypeId in Actions (NULL, 0, or not in Types)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Actions]') AND name = 'TypeId')
BEGIN
    UPDATE [dbo].[Actions] SET [TypeId] = 1 WHERE [TypeId] IS NULL;
    UPDATE [dbo].[Actions] SET [TypeId] = 1 WHERE [TypeId] NOT IN (SELECT [Id] FROM [dbo].[Types]);
END

-- 3) Add FK if column exists but constraint is missing
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Actions]') AND name = 'TypeId')
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Actions_Types_TypeId')
BEGIN
    ALTER TABLE [dbo].[Actions] ADD CONSTRAINT [FK_Actions_Types_TypeId]
    FOREIGN KEY ([TypeId]) REFERENCES [dbo].[Types] ([Id]);
END

-- 4) Add index if missing
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Actions]') AND name = 'TypeId')
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Actions]') AND name = 'IX_Actions_TypeId')
BEGIN
    CREATE INDEX [IX_Actions_TypeId] ON [dbo].[Actions] ([TypeId]);
END
