using System.ComponentModel.DataAnnotations;

namespace RimansaRealEstate.Models
{
    public class Property
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "La ubicación es obligatoria")]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo es obligatorio")]
        public PropertyType Type { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        public PropertyStatus Status { get; set; }

        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? AreaSquareMeters { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public enum PropertyType
    {
        Casa,
        Apartamento,
        Terreno,
        LocalComercial,
        Penthouse,
        Villa
    }

    public enum PropertyStatus
    {
        Venta,
        Alquiler,
        Vendido,
        Alquilado
    }
}