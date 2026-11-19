using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPharmacieApp.Migrations
{
    /// <inheritdoc />
    public partial class AjoutVentesFournisseursHoraires : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HorairesTravail",
                table: "Personnes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroOrdonnance",
                table: "Ordonnances",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "LivraisonsFournisseurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FournisseurId = table.Column<int>(type: "INTEGER", nullable: false),
                    MedicamentCode = table.Column<string>(type: "TEXT", nullable: false),
                    QuantiteRecue = table.Column<int>(type: "INTEGER", nullable: false),
                    DateReception = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LivraisonsFournisseurs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LivraisonsFournisseurs_Fournisseurs_FournisseurId",
                        column: x => x.FournisseurId,
                        principalTable: "Fournisseurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LivraisonsFournisseurs_Medicaments_MedicamentCode",
                        column: x => x.MedicamentCode,
                        principalTable: "Medicaments",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LivraisonsFournisseurs_FournisseurId",
                table: "LivraisonsFournisseurs",
                column: "FournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_LivraisonsFournisseurs_MedicamentCode",
                table: "LivraisonsFournisseurs",
                column: "MedicamentCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LivraisonsFournisseurs");

            migrationBuilder.DropColumn(
                name: "HorairesTravail",
                table: "Personnes");

            migrationBuilder.DropColumn(
                name: "NumeroOrdonnance",
                table: "Ordonnances");
        }
    }
}
