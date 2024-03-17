using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimalAPIPeliculas.Migrations
{
    /// <inheritdoc />
    public partial class addedstatuscodeerrorstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StackTrace",
                table: "Errores",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "StatusCode",
                table: "Errores",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusCode",
                table: "Errores");

            migrationBuilder.AlterColumn<string>(
                name: "StackTrace",
                table: "Errores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
