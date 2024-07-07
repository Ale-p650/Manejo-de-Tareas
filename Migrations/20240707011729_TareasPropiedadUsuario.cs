using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManejodeTareas.Migrations
{
    /// <inheritdoc />
    public partial class TareasPropiedadUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioCreacionId",
                table: "Tareas",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tareas_UsuarioCreacionId",
                table: "Tareas",
                column: "UsuarioCreacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tareas_AspNetUsers_UsuarioCreacionId",
                table: "Tareas",
                column: "UsuarioCreacionId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tareas_AspNetUsers_UsuarioCreacionId",
                table: "Tareas");

            migrationBuilder.DropIndex(
                name: "IX_Tareas_UsuarioCreacionId",
                table: "Tareas");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Tareas");
        }
    }
}
