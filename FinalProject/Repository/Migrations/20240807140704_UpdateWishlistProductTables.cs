using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    public partial class UpdateWishlistProductTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wishlist_AspNetUsers_AppUserId",
                table: "Wishlist");

            migrationBuilder.DropForeignKey(
                name: "FK_WishlistProduct_Products_ProductId",
                table: "WishlistProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_WishlistProduct_Wishlist_WishlistId",
                table: "WishlistProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WishlistProduct",
                table: "WishlistProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Wishlist",
                table: "Wishlist");

            migrationBuilder.RenameTable(
                name: "WishlistProduct",
                newName: "WishlistProducts");

            migrationBuilder.RenameTable(
                name: "Wishlist",
                newName: "Wishlists");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistProduct_WishlistId",
                table: "WishlistProducts",
                newName: "IX_WishlistProducts_WishlistId");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistProduct_ProductId",
                table: "WishlistProducts",
                newName: "IX_WishlistProducts_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Wishlist_AppUserId",
                table: "Wishlists",
                newName: "IX_Wishlists_AppUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WishlistProducts",
                table: "WishlistProducts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wishlists",
                table: "Wishlists",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistProducts_Products_ProductId",
                table: "WishlistProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistProducts_Wishlists_WishlistId",
                table: "WishlistProducts",
                column: "WishlistId",
                principalTable: "Wishlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_AspNetUsers_AppUserId",
                table: "Wishlists",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WishlistProducts_Products_ProductId",
                table: "WishlistProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_WishlistProducts_Wishlists_WishlistId",
                table: "WishlistProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_AspNetUsers_AppUserId",
                table: "Wishlists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Wishlists",
                table: "Wishlists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WishlistProducts",
                table: "WishlistProducts");

            migrationBuilder.RenameTable(
                name: "Wishlists",
                newName: "Wishlist");

            migrationBuilder.RenameTable(
                name: "WishlistProducts",
                newName: "WishlistProduct");

            migrationBuilder.RenameIndex(
                name: "IX_Wishlists_AppUserId",
                table: "Wishlist",
                newName: "IX_Wishlist_AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistProducts_WishlistId",
                table: "WishlistProduct",
                newName: "IX_WishlistProduct_WishlistId");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistProducts_ProductId",
                table: "WishlistProduct",
                newName: "IX_WishlistProduct_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wishlist",
                table: "Wishlist",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WishlistProduct",
                table: "WishlistProduct",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlist_AspNetUsers_AppUserId",
                table: "Wishlist",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistProduct_Products_ProductId",
                table: "WishlistProduct",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistProduct_Wishlist_WishlistId",
                table: "WishlistProduct",
                column: "WishlistId",
                principalTable: "Wishlist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
