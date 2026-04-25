using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmazonBestSellersExplorer.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAsinToFavoriteProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Asin",
                table: "FavoriteProducts",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Asin",
                table: "FavoriteProducts");
        }
    }
}
