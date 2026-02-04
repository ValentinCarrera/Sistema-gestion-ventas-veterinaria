using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veterinaria_AppWeb.Migrations
{
    /// <inheritdoc />
    public partial class CambiarIdProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Productos",
                newName: "Id_Producto");

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    Id_venta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MontoTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FormaDePago = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.Id_venta);
                });

            migrationBuilder.CreateTable(
                name: "DetallesVenta",
                columns: table => new
                {
                    Id_detalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_venta = table.Column<int>(type: "int", nullable: false),
                    Id_producto = table.Column<int>(type: "int", nullable: true),
                    Id_servicio = table.Column<int>(type: "int", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesVenta", x => x.Id_detalle);
                    table.ForeignKey(
                        name: "FK_DetallesVenta_Productos_Id_producto",
                        column: x => x.Id_producto,
                        principalTable: "Productos",
                        principalColumn: "Id_Producto");
                    table.ForeignKey(
                        name: "FK_DetallesVenta_Servicio_Id_servicio",
                        column: x => x.Id_servicio,
                        principalTable: "Servicio",
                        principalColumn: "Id_Servicio");
                    table.ForeignKey(
                        name: "FK_DetallesVenta_Ventas_Id_venta",
                        column: x => x.Id_venta,
                        principalTable: "Ventas",
                        principalColumn: "Id_venta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVenta_Id_producto",
                table: "DetallesVenta",
                column: "Id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVenta_Id_servicio",
                table: "DetallesVenta",
                column: "Id_servicio");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVenta_Id_venta",
                table: "DetallesVenta",
                column: "Id_venta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallesVenta");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.RenameColumn(
                name: "Id_Producto",
                table: "Productos",
                newName: "Id");
        }
    }
}
