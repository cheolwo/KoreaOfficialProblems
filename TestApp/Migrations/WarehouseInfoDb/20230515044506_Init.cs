using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestApp.Migrations.WarehouseInfoDb
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WarehouseInfoes",
                columns: table => new
                {
                    WARE_NO = table.Column<string>(type: "varchar(255)", nullable: false),
                    PRESIDENT_NAME = table.Column<string>(type: "longtext", nullable: false),
                    STORAGE_ITEM = table.Column<string>(type: "longtext", nullable: false),
                    FROZEN_AREA = table.Column<string>(type: "longtext", nullable: false),
                    RNUM = table.Column<int>(type: "int", nullable: false),
                    COMPANY_NAME = table.Column<string>(type: "longtext", nullable: false),
                    FROZEN_WING_COUNT = table.Column<string>(type: "longtext", nullable: false),
                    GENERAL_WING_COUNT = table.Column<string>(type: "longtext", nullable: false),
                    GENERAL_AREA = table.Column<string>(type: "longtext", nullable: false),
                    COMPANY_TEL = table.Column<string>(type: "longtext", nullable: false),
                    COMPANY_ADDRESS = table.Column<string>(type: "longtext", nullable: false),
                    STORAGE_AREA = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseInfoes", x => x.WARE_NO);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarehouseInfoes");
        }
    }
}
