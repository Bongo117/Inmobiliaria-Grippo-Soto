using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Web.Models
{
    public class Inmueble
    {
        public int Id { get; set; }

        [Required]
        public string Direccion { get; set; } = string.Empty;

        [Required]
        public string Tipo { get; set; } = string.Empty; // Casa, departamento, etc

        [Range(1, int.MaxValue)]
        public int Ambientes { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Precio { get; set; }

        // Relación con propietario
        [Required]
        public int PropietarioId { get; set; }
        public Propietario Propietario { get; set; } = null!;
    }
}
