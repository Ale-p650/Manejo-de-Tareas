using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManejodeTareas.Migrations
{
    /// <inheritdoc />
    public partial class RolAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF NOT EXISTS(Select Id from AspNetRoles
            where Id = '7ae74ea6-f112-49a0-b81a-2fdca1533895')
            BEGIN
            INSERT INTO AspNetRoles(Id,[Name],[NormalizedName])
            VALUES ('7ae74ea6-f112-49a0-b81a-2fdca1533895','admin','ADMIN')
            END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql
                ("DELETE AspNetRoles Where Id='7ae74ea6-f112-49a0-b81a-2fdca1533895'");
        }
    }
}
