using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenantFlowERP.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransaction_Products_ProductId",
                table: "InventoryTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransaction_Tenants_TenantId",
                table: "InventoryTransaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryTransaction",
                table: "InventoryTransaction");

            migrationBuilder.RenameTable(
                name: "InventoryTransaction",
                newName: "InventoryTransactions");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryTransaction_TenantId",
                table: "InventoryTransactions",
                newName: "IX_InventoryTransactions_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryTransaction_ProductId",
                table: "InventoryTransactions",
                newName: "IX_InventoryTransactions_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryTransactions",
                table: "InventoryTransactions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Products_ProductId",
                table: "InventoryTransactions",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Tenants_TenantId",
                table: "InventoryTransactions",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Products_ProductId",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Tenants_TenantId",
                table: "InventoryTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryTransactions",
                table: "InventoryTransactions");

            migrationBuilder.RenameTable(
                name: "InventoryTransactions",
                newName: "InventoryTransaction");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryTransactions_TenantId",
                table: "InventoryTransaction",
                newName: "IX_InventoryTransaction_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryTransactions_ProductId",
                table: "InventoryTransaction",
                newName: "IX_InventoryTransaction_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryTransaction",
                table: "InventoryTransaction",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransaction_Products_ProductId",
                table: "InventoryTransaction",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransaction_Tenants_TenantId",
                table: "InventoryTransaction",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
