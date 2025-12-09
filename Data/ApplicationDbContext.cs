using Microsoft.EntityFrameworkCore;
using RimansaRealEstate.Models;

namespace RimansaRealEstate.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Property> Properties { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // Suprimir la advertencia de cambios pendientes en .NET 9
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de precisión decimal
            modelBuilder.Entity<Property>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Property>()
                .Property(p => p.AreaSquareMeters)
                .HasPrecision(18, 2);

            // Seed inicial - Admin por defecto
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    FullName = "Administrador Rimansa",
                    Email = "admin@rimansa.com",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                }
            );

            // Seed de propiedades de ejemplo
            modelBuilder.Entity<Property>().HasData(
                new Property
                {
                    Id = 1,
                    Title = "Casa Moderna en Los Jardines",
                    Description = "Hermosa casa moderna con acabados de primera calidad, amplia sala-comedor, cocina equipada y jardín.",
                    Price = 250000,
                    Location = "Santo Domingo Norte",
                    Type = PropertyType.Casa,
                    Status = PropertyStatus.Venta,
                    Bedrooms = 3,
                    Bathrooms = 2,
                    AreaSquareMeters = 180,
                    ImageUrl = "/images/properties/casa1.jpg",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new Property
                {
                    Id = 2,
                    Title = "Apartamento de Lujo Vista al Mar",
                    Description = "Espectacular apartamento con vista panorámica al mar, balcón amplio, y todas las amenidades.",
                    Price = 1200,
                    Location = "Malecón, Santo Domingo",
                    Type = PropertyType.Apartamento,
                    Status = PropertyStatus.Alquiler,
                    Bedrooms = 2,
                    Bathrooms = 2,
                    AreaSquareMeters = 120,
                    ImageUrl = "/images/properties/apto1.jpg",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                },
                new Property
                {
                    Id = 3,
                    Title = "Terreno Residencial Premium",
                    Description = "Terreno en zona exclusiva de Bávaro, listo para construir, con todos los servicios disponibles.",
                    Price = 85000,
                    Location = "Bávaro, Punta Cana",
                    Type = PropertyType.Terreno,
                    Status = PropertyStatus.Venta,
                    AreaSquareMeters = 500,
                    ImageUrl = "/images/properties/terreno1.jpg",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                }
            );
        }
    }
}