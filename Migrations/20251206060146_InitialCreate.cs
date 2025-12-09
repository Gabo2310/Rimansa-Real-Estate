using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RimansaRealEstate.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Bedrooms = table.Column<int>(type: "int", nullable: true),
                    Bathrooms = table.Column<int>(type: "int", nullable: true),
                    AreaSquareMeters = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "LastLogin", "PasswordHash", "Username" },
                values: new object[] { 1, new DateTime(2025, 12, 6, 2, 1, 46, 179, DateTimeKind.Local).AddTicks(360), "admin@rimansa.com", "Administrador Rimansa", true, null, "$2a$11$Qy2ew.EE5P.D7DLSbAcTeO.ylL0fxU1Ctm/kY5H.qZxCrJBbrHQQq", "admin" });

            migrationBuilder.InsertData(
                table: "Properties",
                columns: new[] { "Id", "AreaSquareMeters", "Bathrooms", "Bedrooms", "CreatedAt", "Description", "ImageUrl", "IsActive", "Location", "Price", "Status", "Title", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 180m, 2, 3, new DateTime(2025, 12, 6, 2, 1, 46, 180, DateTimeKind.Local).AddTicks(1808), "Hermosa casa moderna con acabados de primera calidad, amplia sala-comedor, cocina equipada y jardín.", "/images/properties/casa1.jpg", true, "Santo Domingo Norte", 250000m, 0, "Casa Moderna en Los Jardines", 0, null },
                    { 2, 120m, 2, 2, new DateTime(2025, 12, 6, 2, 1, 46, 180, DateTimeKind.Local).AddTicks(2141), "Espectacular apartamento con vista panorámica al mar, balcón amplio, y todas las amenidades.", "/images/properties/apto1.jpg", true, "Malecón, Santo Domingo", 1200m, 1, "Apartamento de Lujo Vista al Mar", 1, null },
                    { 3, 500m, null, null, new DateTime(2025, 12, 6, 2, 1, 46, 180, DateTimeKind.Local).AddTicks(2145), "Terreno en zona exclusiva de Bávaro, listo para construir, con todos los servicios disponibles.", "/images/properties/terreno1.jpg", true, "Bávaro, Punta Cana", 85000m, 0, "Terreno Residencial Premium", 2, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Properties");
        }
    }
}
