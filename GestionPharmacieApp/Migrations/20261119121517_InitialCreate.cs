using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionPharmacieApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fournisseurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Telephone = table.Column<string>(type: "TEXT", nullable: false),
                    Adresse = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fournisseurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lots",
                columns: table => new
                {
                    NumeroLot = table.Column<string>(type: "TEXT", nullable: false),
                    DateExpiration = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Quantite = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lots", x => x.NumeroLot);
                });

            migrationBuilder.CreateTable(
                name: "Personnes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Prenom = table.Column<string>(type: "TEXT", nullable: false),
                    Telephone = table.Column<string>(type: "TEXT", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    Adresse = table.Column<string>(type: "TEXT", nullable: true),
                    NumeroCnam = table.Column<string>(type: "TEXT", nullable: true),
                    Specialite = table.Column<string>(type: "TEXT", nullable: true),
                    NumeroOrdre = table.Column<string>(type: "TEXT", nullable: true),
                    Pharmacien_NumeroOrdre = table.Column<string>(type: "TEXT", nullable: true),
                    Role = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personnes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Medicaments",
                columns: table => new
                {
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Categorie = table.Column<string>(type: "TEXT", nullable: false),
                    SurOrdonnance = table.Column<bool>(type: "INTEGER", nullable: false),
                    FournisseurId = table.Column<int>(type: "INTEGER", nullable: true),
                    PrixUnitaire = table.Column<decimal>(type: "TEXT", nullable: false),
                    SeuilAlerte = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicaments", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Medicaments_Fournisseurs_FournisseurId",
                        column: x => x.FournisseurId,
                        principalTable: "Fournisseurs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Ordonnances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateEmission = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MedecinId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ordonnances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ordonnances_Personnes_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Personnes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ordonnances_Personnes_MedecinId",
                        column: x => x.MedecinId,
                        principalTable: "Personnes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LignesOrdonnance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Quantite = table.Column<int>(type: "INTEGER", nullable: false),
                    Posologie = table.Column<string>(type: "TEXT", nullable: false),
                    MedicamentCode = table.Column<string>(type: "TEXT", nullable: false),
                    OrdonnanceId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LignesOrdonnance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LignesOrdonnance_Medicaments_MedicamentCode",
                        column: x => x.MedicamentCode,
                        principalTable: "Medicaments",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LignesOrdonnance_Ordonnances_OrdonnanceId",
                        column: x => x.OrdonnanceId,
                        principalTable: "Ordonnances",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Ventes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateVente = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModePaiement = table.Column<string>(type: "TEXT", nullable: false),
                    Validee = table.Column<bool>(type: "INTEGER", nullable: false),
                    PharmacienId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: true),
                    OrdonnanceId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ventes_Ordonnances_OrdonnanceId",
                        column: x => x.OrdonnanceId,
                        principalTable: "Ordonnances",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Ventes_Personnes_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Personnes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Ventes_Personnes_PharmacienId",
                        column: x => x.PharmacienId,
                        principalTable: "Personnes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LignesVente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Quantite = table.Column<int>(type: "INTEGER", nullable: false),
                    PrixUnitaire = table.Column<decimal>(type: "TEXT", nullable: false),
                    MedicamentCode = table.Column<string>(type: "TEXT", nullable: false),
                    VenteId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LignesVente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LignesVente_Medicaments_MedicamentCode",
                        column: x => x.MedicamentCode,
                        principalTable: "Medicaments",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LignesVente_Ventes_VenteId",
                        column: x => x.VenteId,
                        principalTable: "Ventes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LignesOrdonnance_MedicamentCode",
                table: "LignesOrdonnance",
                column: "MedicamentCode");

            migrationBuilder.CreateIndex(
                name: "IX_LignesOrdonnance_OrdonnanceId",
                table: "LignesOrdonnance",
                column: "OrdonnanceId");

            migrationBuilder.CreateIndex(
                name: "IX_LignesVente_MedicamentCode",
                table: "LignesVente",
                column: "MedicamentCode");

            migrationBuilder.CreateIndex(
                name: "IX_LignesVente_VenteId",
                table: "LignesVente",
                column: "VenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicaments_FournisseurId",
                table: "Medicaments",
                column: "FournisseurId");

            migrationBuilder.CreateIndex(
                name: "IX_Ordonnances_ClientId",
                table: "Ordonnances",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Ordonnances_MedecinId",
                table: "Ordonnances",
                column: "MedecinId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventes_ClientId",
                table: "Ventes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventes_OrdonnanceId",
                table: "Ventes",
                column: "OrdonnanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventes_PharmacienId",
                table: "Ventes",
                column: "PharmacienId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LignesOrdonnance");

            migrationBuilder.DropTable(
                name: "LignesVente");

            migrationBuilder.DropTable(
                name: "Lots");

            migrationBuilder.DropTable(
                name: "Medicaments");

            migrationBuilder.DropTable(
                name: "Ventes");

            migrationBuilder.DropTable(
                name: "Fournisseurs");

            migrationBuilder.DropTable(
                name: "Ordonnances");

            migrationBuilder.DropTable(
                name: "Personnes");
        }
    }
}
