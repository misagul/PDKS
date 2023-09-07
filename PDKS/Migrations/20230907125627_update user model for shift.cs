using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PDKS.Migrations
{
    /// <inheritdoc />
    public partial class updateusermodelforshift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Shift",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Shift",
                table: "Users");
        }
    }
}
