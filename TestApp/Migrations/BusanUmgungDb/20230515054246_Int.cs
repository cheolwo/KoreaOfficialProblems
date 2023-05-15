using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestApp.Migrations.BusanUmgungDb
{
    /// <inheritdoc />
    public partial class Int : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BusanUmgungInfos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    midName = table.Column<string>(type: "longtext", nullable: false),
                    goodName = table.Column<string>(type: "longtext", nullable: false),
                    danq = table.Column<string>(type: "longtext", nullable: false),
                    dan = table.Column<string>(type: "longtext", nullable: false),
                    poj = table.Column<string>(type: "longtext", nullable: false),
                    sizeName = table.Column<string>(type: "longtext", nullable: false),
                    lv = table.Column<string>(type: "longtext", nullable: false),
                    minCost = table.Column<string>(type: "longtext", nullable: false),
                    maxCost = table.Column<string>(type: "longtext", nullable: false),
                    aveCost = table.Column<string>(type: "longtext", nullable: false),
                    saledate = table.Column<string>(type: "longtext", nullable: false),
                    cmpName = table.Column<string>(type: "longtext", nullable: false),
                    largeName = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusanUmgungInfos", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusanUmgungInfos");
        }
    }
}
