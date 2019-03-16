using Microsoft.EntityFrameworkCore.Migrations;

namespace GraniteHouse.Data.Migrations
{
    public partial class ProductModelUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_SpecialTags_SpesialTagsId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SpesialTagsId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SpesialTagsId",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SpecialTagsId",
                table: "Products",
                column: "SpecialTagsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SpecialTags_SpecialTagsId",
                table: "Products",
                column: "SpecialTagsId",
                principalTable: "SpecialTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_SpecialTags_SpecialTagsId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SpecialTagsId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "SpesialTagsId",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SpesialTagsId",
                table: "Products",
                column: "SpesialTagsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SpecialTags_SpesialTagsId",
                table: "Products",
                column: "SpesialTagsId",
                principalTable: "SpecialTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
