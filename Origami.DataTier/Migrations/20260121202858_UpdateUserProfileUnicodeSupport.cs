using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Origami.DataTier.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserProfileUnicodeSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update display_name to nvarchar(255) for Unicode support (Vietnamese characters)
            migrationBuilder.Sql(@"
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
            ");

            // Update bio to nvarchar(max) for Unicode support (Vietnamese characters)
            migrationBuilder.Sql(@"
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
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
