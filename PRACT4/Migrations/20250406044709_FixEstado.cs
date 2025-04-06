using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRACT4.Migrations
{
    /// <inheritdoc />
    public partial class FixEstado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Estados_Eventos_EventoId",
                table: "Estados");

            migrationBuilder.DropIndex(
                name: "IX_Estados_EventoId",
                table: "Estados");

            migrationBuilder.DropColumn(
                name: "EventoId",
                table: "Estados");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_EstadoId",
                table: "Eventos",
                column: "EstadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Estados_EstadoId",
                table: "Eventos",
                column: "EstadoId",
                principalTable: "Estados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Estados_EstadoId",
                table: "Eventos");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_EstadoId",
                table: "Eventos");

            migrationBuilder.AddColumn<int>(
                name: "EventoId",
                table: "Estados",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estados_EventoId",
                table: "Estados",
                column: "EventoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Estados_Eventos_EventoId",
                table: "Estados",
                column: "EventoId",
                principalTable: "Eventos",
                principalColumn: "Id");
        }
    }
}
