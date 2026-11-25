using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loja_Api.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    ID_USUARIO = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    _nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    _email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    _senha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    _cpf = table.Column<long>(type: "bigint", nullable: false),
                    _telefone = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.ID_USUARIO);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
