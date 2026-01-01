using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCustomerEmailLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Customers]', N'U') IS NOT NULL
    AND COL_LENGTH(N'[dbo].[Customers]', N'CustomerEmail') IS NOT NULL
BEGIN
    DECLARE @df sysname;

    SELECT @df = dc.name
    FROM sys.default_constraints dc
    JOIN sys.columns c
      ON dc.parent_object_id = c.object_id
     AND dc.parent_column_id = c.column_id
    WHERE dc.parent_object_id = OBJECT_ID(N'[dbo].[Customers]')
      AND c.name = N'CustomerEmail';

    IF @df IS NOT NULL
        EXEC(N'ALTER TABLE [dbo].[Customers] DROP CONSTRAINT [' + @df + '];');

    ALTER TABLE [dbo].[Customers]
    ALTER COLUMN [CustomerEmail] nvarchar(100) NOT NULL;
END
");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[Customers]', N'U') IS NOT NULL
    AND COL_LENGTH(N'[dbo].[Customers]', N'CustomerEmail') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Customers]
    ALTER COLUMN [CustomerEmail] nvarchar(max) NOT NULL;
END
");
        }

    }
}
