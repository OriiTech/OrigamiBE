BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121202858_UpdateUserProfileUnicodeSupport'
)
BEGIN

                    IF EXISTS (
                        SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'UserProfile' 
                        AND COLUMN_NAME = 'display_name'
                        AND (DATA_TYPE = 'varchar' OR CHARACTER_SET_NAME IS NULL)
                    )
                    BEGIN
                        ALTER TABLE [UserProfile]
                        ALTER COLUMN [display_name] nvarchar(255) NULL;
                    END
                
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121202858_UpdateUserProfileUnicodeSupport'
)
BEGIN

                    IF EXISTS (
                        SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'UserProfile' 
                        AND COLUMN_NAME = 'bio'
                        AND (DATA_TYPE = 'varchar' OR CHARACTER_SET_NAME IS NULL)
                    )
                    BEGIN
                        ALTER TABLE [UserProfile]
                        ALTER COLUMN [bio] nvarchar(max) NULL;
                    END
                
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260121202858_UpdateUserProfileUnicodeSupport'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260121202858_UpdateUserProfileUnicodeSupport', N'8.0.20');
END;
GO

COMMIT;
GO

