using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _8Boys.Migrations
{
    /// <inheritdoc />
    public partial class updateProductVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "ProductVariants",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RealPrice",
                table: "ProductVariants",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "RealPrice",
                table: "ProductVariants");
        }
    }
}
