using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Web.Models
{
    public class Contrato
    {
        public int Id { get; set; }

        [Required, Display(Name = "Fecha de inicio")]
        public DateTime FechaInicio { get; set; }

        [Required, Display(Name = "Fecha de fin")]
        public DateTime FechaFin { get; set; }

        [Range(0, double.MaxValue)]
        public decimal MontoMensual { get; set; }

        // Relacion con inquilino
        [Required]
        public int InquilinoId { get; set; }
        public Inquilino Inquilino { get; set; } = null!;

        // Relacion con inmueble
        [Required]
        public int InmuebleId { get; set; }
        public Inmueble Inmueble { get; set; } = null!;
    }
}
