-- Run this in SSMS on CleanTEETH_2 if migration 20260302173455_type failed partway.
-- It removes the partial changes so you can run update-database again with the fixed migration.

-- Drop FK (if it was created - in your case it failed, so skip)
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Actions_Types_TypeId')
    ALTER TABLE [dbo].[Actions] DROP CONSTRAINT [FK_Actions_Types_TypeId];

-- Drop index
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Actions]') AND name = 'IX_Actions_TypeId')
    DROP INDEX [IX_Actions_TypeId] ON [dbo].[Actions];

-- Drop column
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Actions]') AND name = 'TypeId')
    ALTER TABLE [dbo].[Actions] DROP COLUMN [TypeId];

-- Drop table
IF OBJECT_ID(N'[dbo].[Types]') IS NOT NULL
    DROP TABLE [dbo].[Types];
