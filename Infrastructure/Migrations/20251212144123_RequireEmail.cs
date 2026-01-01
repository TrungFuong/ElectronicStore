using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class RequireEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Customers]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Customers]', N'CustomerEmail') IS NOT NULL
BEGIN
    DECLARE @var sysname;
    SELECT @var = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c]
        ON [d].[parent_column_id] = [c].[column_id]
       AND [d].[parent_object_id] = [c].[object_id]
    WHERE [d].[parent_object_id] = OBJECT_ID(N'[dbo].[Customers]')
      AND [c].[name] = N'CustomerEmail';

    IF @var IS NOT NULL EXEC(N'ALTER TABLE [dbo].[Customers] DROP CONSTRAINT [' + @var + '];');

    ALTER TABLE [dbo].[Customers] ALTER COLUMN [CustomerEmail] nvarchar(100) NOT NULL;
END
");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Customers]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[Customers]', N'CustomerEmail') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Customers] ALTER COLUMN [CustomerEmail] nvarchar(max) NOT NULL;
END
");
        }

    }
}
