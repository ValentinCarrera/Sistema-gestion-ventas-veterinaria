using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Veterinaria_AppWeb.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaTurno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Turnos",
                columns: table => new
                {
                    Id_Turno = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCliente = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Dia = table.Column<DateTime>(type: "date", nullable: false),
                    Horario = table.Column<TimeSpan>(type: "time", nullable: false),
                    Id_Servicio = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turnos", x => x.Id_Turno);
                    table.ForeignKey(
                        name: "FK_Turnos_Servicio_Id_Servicio",
                        column: x => x.Id_Servicio,
                        principalTable: "Servicio",
                        principalColumn: "Id_Servicio",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_Id_Servicio",
                table: "Turnos",
                column: "Id_Servicio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Turnos");
        }
    }
}
