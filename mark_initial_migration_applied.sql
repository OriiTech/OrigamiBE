-- Mark InitialCreate migration as applied in migration history
-- This allows EF Core to skip it and only apply the new UpdateUserProfileUnicodeSupport migration

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260110170548_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260110170548_InitialCreate', N'8.0.20');
    PRINT 'Marked InitialCreate migration as applied';
END
ELSE
BEGIN
    PRINT 'InitialCreate migration already in history';
END
GO
