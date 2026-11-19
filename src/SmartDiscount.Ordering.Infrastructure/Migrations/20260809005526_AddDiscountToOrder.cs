using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartDiscount.Ordering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                schema: "ordering",
                table: "orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PromoCode",
                schema: "ordering",
                table: "orders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                schema: "ordering",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "PromoCode",
                schema: "ordering",
                table: "orders");
        }
    }
}
